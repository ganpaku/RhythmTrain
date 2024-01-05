using System;
using System.Collections.Generic;
using Godot;
  public enum TimingAccuracy { Perfect, OK, Miss }
    public enum NoteType { Whole, Half, Quarter, Eighth,Sixteenth }
    //contains nextBeatTime, BeatLength, NoteType and lastBeatPlayed
  public class NoteInterval
  {
        public double nextBeatTime;
        public double beatLength;
        public NoteType noteType;
        public double lastBeatPlayed;

        public NoteInterval( NoteType noteType, double beatLengthInSeconds)
        {
           nextBeatTime = 0.0;
           lastBeatPlayed =0.0;
            this.noteType = noteType;
            switch(noteType)
            {
                case NoteType.Whole:
                    beatLength = beatLengthInSeconds*4.0;
                    break;
                case NoteType.Half:
                    beatLength = beatLengthInSeconds*2.0;
                    break;
                case NoteType.Quarter:
                    beatLength = beatLengthInSeconds;
                    break;
                case NoteType.Eighth:
                    beatLength = beatLengthInSeconds/2.0;
                    break;
                case NoteType.Sixteenth:
                    beatLength = beatLengthInSeconds/4.0;
                    break;
                default:
                    beatLength = 8.0;
                    break;

            }
        }
  }
public partial class Conductor : Node
{
    //Public variables
    [Export]
    private int InputLagInMilliseconds = 100;
    [Export] private double bpm = 120.0;
    //Beat Signals
    [Signal] public delegate void WholeNoteEventHandler();
    [Signal] public delegate void HalfNoteEventHandler();
    [Signal] public delegate void QuarterNoteEventHandler();
    [Signal] public delegate void EighthNoteEventHandler();
    [Signal] public delegate void SixteenthNoteEventHandler();

    //Private variables


    private AudioStreamPlayer audioStreamPlayer = null;
     private double previousPlaybackPosition = 0.0;
     private double playbackPosition = 0.0;
    private List<NoteInterval> noteIntervals = new List<NoteInterval>();
    public override void _Ready()
    {
        base._Ready();
        var beatLength = CalculateBeatLength();
        audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        noteIntervals.Add(new NoteInterval(NoteType.Whole,beatLength));
        noteIntervals.Add(new NoteInterval(NoteType.Half,beatLength));
        noteIntervals.Add(new NoteInterval(NoteType.Quarter,beatLength));
        noteIntervals.Add(new NoteInterval(NoteType.Eighth,beatLength));
        noteIntervals.Add(new NoteInterval(NoteType.Sixteenth,beatLength));
    }




public override void _Process(double delta)
{
    // Test Area start

    // Test Area end
    playbackPosition = audioStreamPlayer.GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency();
    if (audioStreamPlayer.Playing)
        {
            foreach(NoteInterval noteInterval in noteIntervals)
            {
                if(CheckForBeat(noteInterval)){
                    switch(noteInterval.noteType)
                    {
                        case NoteType.Whole:

                            EmitSignal(SignalName.WholeNote);
                            break;
                        case NoteType.Half:
                            EmitSignal(SignalName.HalfNote);
                            break;
                        case NoteType.Quarter:
                            GD.Print("currentBeat, nextbeat" + " " + playbackPosition.ToString("0.000") + " " + noteInterval.nextBeatTime.ToString("0.000") + " " + noteInterval.beatLength.ToString("0.000"));
                            EmitSignal(SignalName.QuarterNote);
                            break;
                        case NoteType.Eighth:
                            EmitSignal(SignalName.EighthNote);
                            break;
                        case NoteType.Sixteenth:
                            EmitSignal(SignalName.SixteenthNote);
                            break;
                    }

                }
            }
            previousPlaybackPosition = playbackPosition;
        }
}

#region public methods

/// <summary>
/// For quarter notes only. Calculates distance to the next beat with the current playback position
/// </summary>
/// <returns>a double between 0.0 and 1.0 representing the distance to the next beat. min Value when no beat is found</returns>
public double GetBeatProgressQuarter()
{
    var noteInterval = noteIntervals.Find(interval => interval.noteType== NoteType.Quarter);
    return GetBeatProgress(noteInterval);
}
/// <summary>
/// For eighth notes only. Calculates distance to the next beat with the current playback position
/// </summary>
/// <returns>a double between 0.0 and 1.0 representing the distance to the next beat. min Value when no beat is found</returns>
public double GetBeatProgressEighth()
{
    var noteInterval = noteIntervals.Find(interval => interval.noteType== NoteType.Eighth);
    return GetBeatProgress(noteInterval);
}
/// <summary>
/// For half notes only. Calculates distance to the next beat with the current playback position
/// </summary>
/// <returns>a double between 0.0 and 1.0 representing the distance to the next beat. min Value when no beat is found</returns>
public double GetBeatProgressHalf()
{
    var noteInterval = noteIntervals.Find(interval => interval.noteType== NoteType.Half);
    return GetBeatProgress(noteInterval);
}

/// <summary>
/// For whole notes only. Calculates distance to the next beat with the current playback position
/// </summary>
/// <returns>a double between 0.0 and 1.0 representing the distance to the next beat. min Value when no beat is found</returns>
public double GetBeatProgressWhole()
{
    var noteInterval = noteIntervals.Find(interval => interval.noteType== NoteType.Whole);
    return GetBeatProgress(noteInterval);
}

/// <summary>
/// Returns the current timing accuracy, based on the current playback position
/// </summary>
/// <returns></returns>
public TimingAccuracy IsInBeat()
{
    var noteInterval = noteIntervals.Find(interval => interval.noteType== NoteType.Quarter);
    double timeUntilNextBeat = noteInterval.nextBeatTime - playbackPosition;
    double timeSinceLastBeat = audioStreamPlayer.GetPlaybackPosition() - (noteInterval.nextBeatTime - noteInterval.beatLength);
    //adjust for input lag
    timeUntilNextBeat -= InputLagInMilliseconds / 1000.0;
    timeSinceLastBeat -= InputLagInMilliseconds / 1000.0;
    var adjustedHitTime = playbackPosition - InputLagInMilliseconds / 1000.0;
    GD.Print("adjustedHitTime = "+ adjustedHitTime.ToString("0.000") + " " + "nextBeatTime = " + noteInterval.nextBeatTime.ToString("0.000") + " " + "timeUntilNextBeat = " + timeUntilNextBeat.ToString("0.000") + " " + "timeSinceLastBeat = " + timeSinceLastBeat.ToString("0.000") );

    if (Math.Abs(timeUntilNextBeat) < 0.015 || Math.Abs(timeSinceLastBeat) < 0.015) // Perfect timing
    {
        GD.Print("Perfect!");
        return TimingAccuracy.Perfect;
    }
    else if (Math.Abs(timeUntilNextBeat) < 0.04 || Math.Abs(timeSinceLastBeat) < 0.04) // OK timing
    {
        GD.Print("OK!");
        return TimingAccuracy.OK;
    }
    else // Missed timing
    {
        GD.Print("Miss!");
        return TimingAccuracy.Miss;
    }
}

#endregion

#region private methods


private double GetBeatProgress(NoteInterval noteInterval)
{
    var progress = double.MinValue;
    if (noteInterval != null)
    {

        progress = (noteInterval.nextBeatTime - playbackPosition) / noteInterval.beatLength;
    }
    return progress;
}

    private bool CheckForBeat(NoteInterval noteInterval)
    {

        if (playbackPosition >= noteInterval.nextBeatTime)
        {
            noteInterval.lastBeatPlayed = noteInterval.nextBeatTime;
            noteInterval.nextBeatTime += noteInterval.beatLength;
            if (noteInterval.nextBeatTime > audioStreamPlayer.Stream.GetLength())
            {
                noteInterval.nextBeatTime = noteInterval.beatLength;
            }
            return true;

        }
        if (
            playbackPosition < noteInterval.beatLength
            && Math.Abs(noteInterval.nextBeatTime - LastBeatTime(noteInterval)) < 0.01
            && Math.Abs(noteInterval.lastBeatPlayed - noteInterval.nextBeatTime) > 0.01)
        {
            noteInterval.nextBeatTime = noteInterval.beatLength;
            return true;
        }
        return false;
    }
    //Utils

    private double LastBeatTime(NoteInterval noteInterval)
    {
        var audioStreamLength = audioStreamPlayer.Stream.GetLength();
        //examples: if beat length is 0.5 last beat is 8.0 for a 8.25 second audio stream
        //if beat length is 0.25 last beat is 8.25 for a 8.25 second audio stream
        //calculate the last beat time for the current note type
        return  audioStreamLength - (audioStreamLength % noteInterval.beatLength);
    }

    private double CalculateBeatLength()
    {
        return  60.0 / bpm;
    }

    #endregion
}

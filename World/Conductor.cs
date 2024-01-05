using System;
using System.Collections.Generic;
using Godot;
  public enum TimingAccuracy { Perfect, OK, Miss }
    public enum NoteType { Whole, Half, Quarter, Eighth }
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
                    beatLength = beatLengthInSeconds;
                    break;
                case NoteType.Half:
                    beatLength = beatLengthInSeconds/2.0;
                    break;
                case NoteType.Quarter:
                    beatLength = beatLengthInSeconds/4.0;
                    break;
                case NoteType.Eighth:
                    beatLength = beatLengthInSeconds/8.0;
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

    //Beat Signals
    [Signal] public delegate void WholeBeatEventHandler();
    [Signal] public delegate void HalfBeatEventHandler();
    [Signal] public delegate void QuarterBeatEventHandler();
    [Signal] public delegate void EighthBeatEventHandler();

    //Private variables
    [Export] private double bpm = 120.0;

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
    }

    private double CalculateBeatLength()
    {
        return  60.0 / bpm;
    }

   

public override void _Process(double delta)
{
    playbackPosition = audioStreamPlayer.GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency();
    if (audioStreamPlayer.Playing)
        {
            foreach(NoteInterval noteInterval in noteIntervals)
            {
                if(CheckForBeat(noteInterval)){
                    switch(noteInterval.noteType)
                    {
                        case NoteType.Whole:
                            GD.Print("currentBeat, nextbeat" + " " + playbackPosition.ToString("0.000") + " " + noteInterval.nextBeatTime.ToString("0.000") + " " + noteInterval.beatLength.ToString("0.000"));
                            EmitSignal(SignalName.WholeBeat);
                            break;
                        case NoteType.Half:
                            EmitSignal(SignalName.HalfBeat);
                            break;
                        case NoteType.Quarter:
                            EmitSignal(SignalName.QuarterBeat);
                            break;
                        case NoteType.Eighth:
                            EmitSignal(SignalName.EighthBeat);
                            break;
                    }
                    
                }
            }
            previousPlaybackPosition = playbackPosition;
        }
       
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
        else if (playbackPosition < noteInterval.beatLength && noteInterval.nextBeatTime == 8.0 &&noteInterval.lastBeatPlayed != noteInterval.nextBeatTime)
        {
            noteInterval.nextBeatTime = noteInterval.beatLength;
            EmitSignal("WholeBeatEventHandler");
            return true;
        }
        return false;
    }

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
}

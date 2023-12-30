using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

//Different Types of Beats the Game uses
public enum BeatType
{
	Whole, Half, Quarter, Eighth
}

public partial class Conductor : Node
{
	[Export(PropertyHint.Range,"30,200")] private float bpm;

	[Export] private AudioStreamPlayer audioStreamPlayer;

	//Beat Signals
	[Signal] public delegate void WholeBeatEventHandler();
	[Signal] public delegate void HalfBeatEventHandler();
	[Signal] public delegate void QuarterBeatEventHandler();
	[Signal] public delegate void EighthBeatEventHandler();
	
	
	private List<Intervals> intervals = new List<Intervals>();
	public override void _Ready()
	{
		//Eighth
		intervals.Add(new Intervals(BeatType.Eighth));
		//Quarter
		intervals.Add(new Intervals(BeatType.Quarter));
		//Half
		intervals.Add(new Intervals(BeatType.Half));
		//Whole
		intervals.Add(new Intervals(BeatType.Whole));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//interval.GetIntervalLength((float)audioStreamPlayer.Stream._GetBpm()) when using mp3
		foreach (var interval in intervals)
		{
			
			var normalizedTime = (audioStreamPlayer.GetPlaybackPosition()*48000f)/
								 (48000 * interval.GetIntervalLength(bpm));
			bool newBeat = interval.CheckForNewInterval(normalizedTime);
			if (newBeat)
			{
				switch (interval.beatType)
				{
					case BeatType.Whole:
						EmitSignal(SignalName.WholeBeat);
						GD.Print("Whole beat!");
						break;
					case BeatType.Half:
						EmitSignal(SignalName.HalfBeat);
						break;
					case BeatType.Quarter:
						EmitSignal(SignalName.QuarterBeat);
						GD.Print("Quarter beat!");
						break;
					case BeatType.Eighth:
						EmitSignal(SignalName.EighthBeat);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}


/// <summary>
/// Interval class for different kinds of beats, e.g. quarter, half, whole etc.
/// </summary>
public class Intervals
{
	
	[Export] private float steps;
	
	private int lastInterval;
	public readonly BeatType beatType;
	public Intervals(BeatType beatType)
	{
		this.beatType = beatType; 
		switch (beatType)
		{
			case BeatType.Whole:
				steps = 0.25f;
				break;
			case BeatType.Half:
				steps = 0.5f;
				break;
			case BeatType.Quarter:
				steps = 1f;
				break;
			case BeatType.Eighth:
				steps = 2f;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(beatType), beatType, null);
		}
		
	}

	public float GetIntervalLength(float bpm)
	{
		return 60f / (bpm * steps);
	}
	public bool CheckForNewInterval(float interval)
	{
		// If the floor of the normalized time changes, a new beat is detected
		if (Mathf.FloorToInt(interval) != lastInterval)
		{
			lastInterval = Mathf.FloorToInt(interval);
			//Signal
			return true;
		}

		return false;
	}
}

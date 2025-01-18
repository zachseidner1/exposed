using Godot;
using System;
using System.Threading.Tasks;

public partial class Phone : Sprite2D
{
	[Export]
	public AudioStreamPlayer2D PhoneAudio;

	[Export]
	public SoundWave MySoundWave;

	[Export]
	public AudioStream PhoneRing;

	[Export]
	public AudioStream PhonePickup;

	[Export]
	public AudioStream PhoneCall;

	private Timer _ringTimer;

	State PhoneState = State.Idle;

	enum State
	{
		Idle,
		Ringing,
		Answered
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ringTimer = new();
		AddChild(_ringTimer);
		_ringTimer.Timeout += () =>
		{
			PhoneState = State.Ringing;
			PhoneAudio.Stream = PhoneRing;
			PhoneAudio.Play();
			MySoundWave.AnimateWave();
		};
	}

	public void BeginCall()
	{
		PhoneState = State.Ringing;
		_ringTimer.Start(3);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseButton inputEventMouse)
		{
			if (inputEventMouse.Pressed && inputEventMouse.ButtonIndex == MouseButton.Left)
			{
				if (GetRect().HasPoint(ToLocal(inputEventMouse.Position)))
				{
					PickupPhone();
				}
			}
		}
	}

	private void PickupPhone()
	{
		if (PhoneState != State.Ringing)
		{
			return;
		}
		_ringTimer.Stop();
		PhoneAudio.Stop();
		PhoneAudio.Stream = PhonePickup;
		PhoneAudio.Play();
		PhoneAudio.Finished += () =>
		{
			PhoneAudio.Stream = PhoneCall;
			PhoneAudio.VolumeDb = 5;
			PhoneAudio.Play();
		};
	}
}

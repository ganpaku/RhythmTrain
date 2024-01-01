extends RigidBody3D
@onready var lane_markers = $"../LaneMarkers"
var lanes = [Marker3D]
var currentLane
@export var data : Resource
@onready var conductor = %Conductor
@onready var hop_anim = %HopAnim
@onready var beat_anim = %BeatAnim
@onready var player_model = %PlayerModel_cube1
@onready var model_transform = %ModelTransform

# Called when the node enters the scene tree for the first time.
func _ready():
	lanes = lane_markers.get_children()
	print(lanes)
	currentLane = 1
	
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if Input.is_action_just_pressed("ui_right"):
		if currentLane >= lanes.size() -1:
			return
		else:
			currentLane += 1
			move_player(currentLane)
			hop_anim.play("HopRight")
	if Input.is_action_just_pressed("ui_left"):
		if currentLane <= 0:
			return
		else:
			currentLane -= 1
			move_player(currentLane)
			hop_anim.play("HopLeft")
			#global_position = lane_right.global_position
func move_player(index):
	var tween = create_tween()
	tween.tween_property(self, "global_position", lanes[index].global_position, 0.2)
	
func beatAnimTween():
	var tween = create_tween()
	tween.tween_property(model_transform, "scale", Vector3(1.3,1.3,1.3), 0.05).set_ease(Tween.EASE_IN_OUT)
	tween.tween_property(model_transform, "scale", Vector3(1.0,1.0,1.0), 0.05).set_ease(Tween.EASE_OUT)
func _on_conductor_half_beat():
	pass
	#position.y += 4


func _on_conductor_eighth_beat():
	pass

func _on_conductor_quarter_beat():
	#beat_anim.play("EveryBeatSquash")
	beatAnimTween()
	pass # Replace with function body.

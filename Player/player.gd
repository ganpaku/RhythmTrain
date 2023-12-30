extends RigidBody3D
@onready var lane_markers = $"../LaneMarkers"
var lanes = [Marker3D]
var currentLane
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
	if Input.is_action_just_pressed("ui_left"):
		if currentLane <= 0:
			return
		else:
			currentLane -= 1
			move_player(currentLane)
			#global_position = lane_right.global_position
func move_player(index):
	var tween = create_tween()
	tween.tween_property(self, "global_position", lanes[index].global_position, 0.2)
	
	

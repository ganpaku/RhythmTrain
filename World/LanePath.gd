extends Path3D

@onready var path_follow_3d = [PathFollow3D]
@onready var conductor = %Conductor
var beatLength = 0.0
var travelSpeed = 0.0
@export var leftLaneList : Array[String]
@export var middleLaneList : Array
@export var rightLaneList : Array
var obstacle = preload("res://Obstacles/BlockObst.tscn")
var index = 0

func _ready():
	beatLength = conductor.CalculateBeatLength()
	travelSpeed = 1 / beatLength
	self.conductor.connect("QuarterBeat", tweenOnBeat)
	self.conductor.connect("QuarterBeat", spawnObstacle)
	for child in get_children():
		path_follow_3d = child
	
	
	#print(column_data_array)
func _process(delta):
	pass
	
	#path_follow_3d.progress_ratio -= (travelSpeed / 4) * delta
func spawnObstacle():
	if self == %LaneLeft:
		if index >= leftLaneList.size():
			printerr(error_string(ERR_INVALID_DATA))
			return
		if leftLaneList[index].contains("o"):
			var obst_instance = obstacle.instantiate()
			get_child(0).add_child(obst_instance)
		index+=1


func tweenOnBeat():
	pass # Replace with function body.
	var tween = create_tween()
	tween.tween_property(path_follow_3d, "progress_ratio", path_follow_3d.progress_ratio-0.125, beatLength)

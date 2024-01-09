extends Path3D

enum LaneType{LEFT, MIDDLE, RIGHT}
@export var laneType = LaneType.LEFT

@onready var path_follow_array : Array[PathFollow3D]
@onready var conductor = %Conductor
var beatLength = 0.0
var travelSpeed = 0.0
@export var laneList : Array[String]

var obstacle = preload("res://Obstacles/BlockObstPath.tscn")
var beat_marker = preload("res://BeatMarker/BeatMarkerFollow.tscn")
var index = 0

func _ready():

	beatLength = conductor.CalculateBeatLength()
	travelSpeed = 1 / beatLength
	self.conductor.connect("QuarterNote", tweenOnBeat)
	self.conductor.connect("EighthNote", spawnObstacle)
	#for child in get_children():
		#path_follow_array.push_back(child)
		#print(path_follow_array)

	#print(column_data_array)
func _process(delta):
	pass

	#path_follow_array.progress_ratio -= (travelSpeed / 4) * delta

#get called with "QuarterBeat" signal from Conductor
func spawnObstacle():
	if index >= laneList.size():
		#printerr(error_string(ERR_INVALID_DATA))
		return
	if laneList[index].contains("o"):
		var obst_instance = obstacle.instantiate()
		add_child(obst_instance)
		(obst_instance as PathFollow3D).progress_ratio = 1
		path_follow_array.push_back(obst_instance)
		var tween = create_tween()
		tween.tween_property(obst_instance, "progress_ratio", obst_instance.progress_ratio - 0.125, beatLength)

	elif laneList[index].contains("m"):
		var marker_instance = beat_marker.instantiate()
		add_child(marker_instance)
		(marker_instance as PathFollow3D).progress_ratio = 1
		path_follow_array.push_back(marker_instance)
		var tween = create_tween()
		tween.tween_property(marker_instance, "progress_ratio", marker_instance.progress_ratio - 0.125, beatLength)

	index+=1

#gets called with "QuarterBeat" from Conductor
func tweenOnBeat():
	pass # Replace with function body.
	for path in path_follow_array:
		if is_instance_valid(path):
			var tween = create_tween()
			# tweens the progress_ratio of the path_follow by the beat length. 
			# 0.125 is 1/8 meaning the object on the path will travel an eigth of the path every beat
			# so the object will reach the player in 8 beats. 
			# 0.0355 is the point on the path where the players transform is. 
			# an eigth of this will be substracted on every tween to compensate for the "leftover" path behind the player
			tween.tween_property(path, "progress_ratio", (path.progress_ratio + (0.0355/8)) - 0.125, beatLength)

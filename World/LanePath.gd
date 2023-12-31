extends Path3D

enum LaneType{LEFT, MIDDLE, RIGHT}

@onready var path_follow_array : Array[PathFollow3D]
@onready var conductor = %Conductor
var beatLength = 0.0
@export var laneList : Array[String]

var obstacle = preload("res://Obstacles/BlockObstPath.tscn")
var beat_marker = preload("res://BeatMarker/BeatMarkerFollow.tscn")
var index = 0

func _ready():

	beatLength = conductor.CalculateBeatLength()
	self.conductor.connect("EighthNote", tweenOnBeat)
	self.conductor.connect("EighthNote", spawnObstacle)
	#for child in get_children():
		#path_follow_array.push_back(child)
		#print(path_follow_array)

	#print(column_data_array)
func _process(delta):
	pass

	#path_follow_array.progress_ratio -= (travelSpeed / 4) * delta

#get called with "EighthNote" signal from Conductor
func spawnObstacle():
	if index >= laneList.size():
		#printerr(error_string(ERR_INVALID_DATA))
		return
	if laneList[index].contains("o"):
		var obst_instance = obstacle.instantiate()
		add_child(obst_instance,true)
		(obst_instance as PathFollow3D).progress_ratio = 1.0 
		path_follow_array.push_back(obst_instance)
		

	elif laneList[index].contains("m"):
		var marker_instance = beat_marker.instantiate()
		add_child(marker_instance,true)
		
		#print("SPAWN", Time.get_ticks_msec())
		(marker_instance as PathFollow3D).progress_ratio = 1.0 
		path_follow_array.push_back(marker_instance)
		
	index+=1

#gets called with "QuarterBeat" from Conductor
func tweenOnBeat():
	for path in path_follow_array:
		if is_instance_valid(path):
			var followObject = (path as PathFollowObject)
			if(followObject):
				followObject.advanceBeat(beatLength/2)
				#var tween =  create_tween()
				#tween.tween_property(path, "progress_ratio", (path.progress_ratio + (0.0622/8)) - 0.125, beatLength)
			


func _on_conductor_eighth_note():
	spawnObstacle()

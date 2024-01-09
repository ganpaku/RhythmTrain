extends PathFollow3D
class_name BeatMarkerFollow
var beat =1;
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	pass

func advanceBeat(beatLength):
	beat+=1
	var tween = create_tween()
	var playerPos = 0.062
	var trackLength = 1-playerPos
	var distancePerBeat =trackLength/8 #2 bars of quarter
	var targetPos = 1.0 - distancePerBeat * beat
	tween.tween_property(self, "progress_ratio", targetPos, beatLength)
	


func _on_area_3d_area_entered(area):
	if area.is_in_group("DeleteArea"):
		queue_free()

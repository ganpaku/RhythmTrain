extends PathFollow3D
class_name PathFollowObject
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
	var playerPos = 0.0622
	var trackLength = 1-playerPos
	var distancePerBeat =trackLength/16 #2 bars of quarter
	var targetPos = 1.0 - distancePerBeat * beat
	tween.tween_property(self, "progress_ratio", targetPos, beatLength)

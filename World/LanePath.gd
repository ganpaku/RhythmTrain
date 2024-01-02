extends Path3D

@onready var path_follow_3d = $PathFollow3D

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	path_follow_3d.progress -= 30 * delta

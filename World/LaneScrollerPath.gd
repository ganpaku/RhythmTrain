extends Path3D
var lanes = []

# Called when the node enters the scene tree for the first time.
func _ready():
	lanes = get_children()
	print(lanes)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	for child in lanes:
		child.progress_ratio -= 0.125 * delta

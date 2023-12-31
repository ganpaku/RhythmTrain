extends RigidBody3D
@onready var lane_markers = $"../LaneMarkers"
var lanes = [Marker3D]
var currentLane
@export var data : Resource
@onready var conductor = $"../Conductor"
@onready var hop_anim = %HopAnim
@onready var beat_anim = %BeatAnim
@onready var player_model = %PlayerModel_cube1
@onready var model_transform = %ModelTransform
@onready var ring_particles = %RingParticle
@onready var note_hitbox = %NoteHitbox


@export var maxHealth = 3
@export var invincible = false
var health
var isHopping = false
signal noHealth
signal buttonPressed
# Called when the node enters the scene tree for the first time.
func _ready():
	health = maxHealth
	lanes = lane_markers.get_children()
	print(lanes)
	currentLane = 1
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if Input.is_action_just_pressed("ui_up"):
		var beatProgress = conductor.GetBeatProgressQuarter()
	if Input.is_action_just_pressed("ui_right"):
		if note_hitbox.has_overlapping_areas():
			print("HITT")
		if currentLane >= lanes.size() -1:
			return
		elif !isHopping:
			currentLane += 1
			move_player(currentLane)
			hop_anim.play("HopRight")
		emit_signal("buttonPressed")
	if Input.is_action_just_pressed("ui_left"):
		if note_hitbox.has_overlapping_areas():
			print("HITT")
		if currentLane <= 0:
			return
		elif !isHopping:
			currentLane -= 1
			move_player(currentLane)
			hop_anim.play("HopLeft")
			#global_position = lane_right.global_position
		emit_signal("buttonPressed")
func move_player(index):
	var tween = create_tween()
	isHopping = true
	tween.tween_property(self, "global_position", lanes[index].global_position, 0.15)
	await tween.finished
	isHopping = false

func beatAnimTween():
	var tween = create_tween()
	tween.tween_property(model_transform, "scale", Vector3(1.3,1.3,1.3), 0.05).set_ease(Tween.EASE_IN_OUT)
	tween.tween_property(model_transform, "scale", Vector3(1.0,1.0,1.0), 0.05).set_ease(Tween.EASE_OUT)


func _on_conductor_quarter_beat():
	beatAnimTween()
	ring_particles.emitting = true
	pass # Replace with function body.

func _on_no_health():
	queue_free()

func _on_hurtbox_area_entered(area):
	if !invincible:
		health -= 1
		print(health)
		if health <= 0:
			emit_signal("noHealth")

func _on_conductor_quarter_note():
	beatAnimTween()
	ring_particles.emitting = true

func _on_conductor_whole_note():
	beatAnimTween()
	ring_particles.emitting = true



func _on_button_pressed():
	pass
	var tween = create_tween()
	tween.tween_property(%PlayerCamera,"fov", 95, 0.1)
	tween.tween_property(%PlayerCamera,"fov", 100, 0.1)

func checkBeatHit():
	pass
	#if note_hitbox.has_overlapping_areas():
		#print("HITTTTTT")


from flask import Flask, render_template
from flask_socketio import SocketIO

app = Flask(__name__)
app.config['SECRET_KEY'] = 'SECRET_KEY'
app.config['ENV'] = 'production'
socketio = SocketIO(app)
eeg_events = [
	{
		'sesssion_id': '8a6s',
		'interest': 0.8,
		'stress': 0.8,
		'relaxation': 0.8,
		'excitement': 0.8,
		'engagement': 0.8,
		'focus': 0.8
	}
]
session_ids = []


@socketio.on('eeg_event_post')
def handle_eeg_post_event(json):
	eeg_events.append(json)
	print('received eeg_event: ' + str(json))


@socketio.on('eeg_event_get')
def handle_eeg_get_event(session_id):
	events = []
	eeg_events_copy = eeg_events
	for eeg_event in eeg_events_copy:
		if session_id == eeg_event['sesssion_id']:
			events.append(eeg_event)
			eeg_events.remove(eeg_event)
		else:
			continue

	if events:
		return events
	else:
		return False


@socketio.on('new_game_event')
def handle_new_game_event(session_id):

	for eeg_event in eeg_events:
		if session_id == eeg_event['sesssion_id']:
			return True, eeg_event
		else:
			continue
	return False


@socketio.on('end_game_event')
def handle_end_game_event(json):
	print('received end_game_event: ' + str(json))


if __name__ == '__main__':
	socketio.run(app, host='0.0.0.0', port=5000)

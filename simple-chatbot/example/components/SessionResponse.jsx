import React from 'react';
import { Loading } from  '../../lib/index';
import socketIOClient from "socket.io-client";


export default class SessionResponse extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      loading: true,
      result: '',
      trigger: false,
    };

    this.triggetNext = this.triggetNext.bind(this);
  }

  componentDidMount () {
    const self = this;
    const { steps } = this.props;
    const session_id = steps.session_id.value;

    
    let ioClient = socketIOClient.connect("http://localhost:5000");

    ioClient.on('connect', () => {
      ioClient.emit('new_game_event', session_id, (state, message) => {
          console.log(state);

          if(state == true){
            this.triggetNext();
            this.setState(
              {
                loading: false,
                result: "Synchronization was successful"
              }
            )
          }
          if(state == false){
            this.setState(
              {
                loading: false,
                result: "Synchronization failed"
              }
            )
          }
          console.log(message);
      });
    });


    console.log(session_id);

  }

  triggetNext() {
    this.setState({ trigger: true }, () => {
      this.props.triggerNextStep();
    });
  }

  render() {
    const {  loading, result } = this.state;

    return (
      <div>
        { loading ? <Loading /> : result }
      </div>
    );
  }
}

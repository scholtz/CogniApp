import React, { PureComponent } from "react";
import { w3cwebsocket as W3CWebSocket } from "websocket";

const MIN_X = 12,
  MIN_Y = 12,
  MAX_X = window.innerWidth - MIN_X,
  MAX_Y = window.innerHeight - MIN_Y,
  SPEED = 30,
  WIN_X = window.innerWidth
  ;


export default class Ball extends PureComponent {
  state = {
    x: MIN_X,
    y: MIN_Y,
    position : "absolute",
    top: window.innerHeight/2 + "px",
    left: 200 + "px",
    directionx: 1,
    directiony: 0,
    focus: 0
  };
   animatedStyle = {
    };
    
  componentDidMount() {
    const x = Math.floor(Math.random() * SPEED);
    const y = SPEED - x;
    this.animate();

    const client = new W3CWebSocket('ws://scholtz.sk:5000');
    
    client.onopen = () => {
      console.log('WebSocket Client Connected');
    };
    client.onmessage = (message) => {
        try{
            let dataFromServer = JSON.parse(message.data);
            if(dataFromServer["focus"] !== undefined){
                //var newstate = this.state;
                
                
                  var newstate = {};
                  
                  newstate.x = this.state.x;
                  newstate.y = this.state.y;
                  newstate.left = this.state.left;
                  newstate.top = this.state.top;
                  newstate.focus = dataFromServer["focus"];
                  newstate.directionx = this.state.directionx;
                  newstate.directiony = this.state.directiony;
                
                this.setState(newstate);
                console.log("after new data",newstate);
            }
            console.log(dataFromServer);
        }catch(e){
                console.log("error",e,message);
        }
    };
  }

  animate = () => {
    const { x, y, top, left, directionx, directiony, focus } = this.state;
    if (directionx !== 0 || directiony !== 0) {
        let newX = x + focus;
        if(x === undefined){
            newX = x + focus;
        }
      

      if(newX > window.innerWidth - 300){
          // win
      }else{
          //play
          var newstate = {};
          
          newstate.x = newX;
          newstate.y = this.state.y;
          newstate.left = newX +"px";
          newstate.top = window.innerHeight/2 - 50 + "px";
          newstate.focus = focus;
          newstate.directionx = directionx;
          newstate.directiony = directiony;
          
          this.setState(newstate);
      }

    }
    
    this.animationTimeout = setTimeout(this.animate, 20);
  };

  render() {
    
    return (
      <img src="https://apps.scholtz.sk/ball.png" style={this.state} />
    );
  }

  componentWillUnmount() {
    clearTimeout(this.animationTimeout);
  }
}

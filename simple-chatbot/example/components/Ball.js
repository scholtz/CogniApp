import React, { PureComponent } from "react";
import Konva from "konva";
import { Circle } from "react-konva";
import { WIDTH, HEIGHT } from "./Field";

const MIN_X = 12,
  MIN_Y = 12,
  MAX_X = WIDTH - MIN_X,
  MAX_Y = HEIGHT - MIN_Y,
  SPEED = 30;

export default class Ball extends PureComponent {
  state = {
    color: Konva.Util.getRandomColor(),
    x: MIN_X,
    y: MIN_Y,
    direction: { x: 0, y: 0 }
  };

  componentDidMount() {
    const x = Math.floor(Math.random() * SPEED);
    const y = SPEED - x;
    this.setState({ direction: { x, y } });
    this.animate();
  }

  newCoord = (val, delta, max, min) => {
    let newVal = val + delta;
    let newDelta = delta;

    if (newVal > max || newVal < min) {
      newDelta = -delta;
    }

    if (newVal < min) {
      newVal = min - newVal;
    }
    if (newVal > max) {
      newVal = newVal - (newVal - max);
    }

    return { val: newVal, delta: newDelta };
  };

  animate = () => {
    const { direction, x, y } = this.state;

    if (direction.x !== 0 || direction.y !== 0) {
      const newX = this.newCoord(x, direction.x, MAX_X, MIN_X);
      const newY = this.newCoord(y, direction.y, MAX_Y, MIN_Y);

      this.setState({
        x: newX.val,
        y: newY.val,
        direction: {
          x: newX.delta,
          y: newY.delta
        }
      });
    }

    this.animationTimeout = setTimeout(this.animate, 50);
  };

  render() {
    const { color, x, y } = this.state;

    return (
      <Circle
        ref={comp => {
          this.ball = comp;
        }}
        x={x}
        y={y}
        radius={20}
        fill={color}
        //shadowBlur={1}
      />
    );
  }

  componentWillUnmount() {
    clearTimeout(this.animationTimeout);
  }
}

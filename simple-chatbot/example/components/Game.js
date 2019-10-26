import React, { Component } from "react";
import { Layer, Circle, Rect } from "react-konva";
import Field from "./Field";
import Ball from "./Ball";

export default class Game extends Component {
  render() {
    return (
      <Layer>
        <Field />
        <Ball />
      </Layer>
    );
  }
}
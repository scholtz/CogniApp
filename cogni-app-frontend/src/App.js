import React, { Component } from 'react';
import { render } from 'react-dom';
import Ball from './Ball';

export default class App extends Component {
  render() {
    return (
    <div>
      <img src="https://apps.scholtz.sk/field.jpg" alt="field" width={window.innerWidth} height={window.innerHeight}/> 
      <Ball/>
      </div>
    );
  }
}

render(<App />, document.getElementById('root'));

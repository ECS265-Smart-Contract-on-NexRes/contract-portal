import React, { Component } from 'react';
import { FileInput } from './FileInput';

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div>
        <h1>Upload a smart contract binary</h1>
        <FileInput></FileInput>
      </div>
    );
  }
}

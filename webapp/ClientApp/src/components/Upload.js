import React, { Component } from 'react';
import { FileInput } from './FileInput';

export class Upload extends Component {
  static displayName = Upload.name;

  render() {
    return (
      <div>
        <h1>Upload a smart contract</h1>
        <FileInput></FileInput>
      </div>
    );
  }
}

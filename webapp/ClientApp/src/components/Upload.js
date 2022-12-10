import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { FileInput } from './FileInput';

export class Upload extends Component {
  static displayName = Upload.name;

  render() {
    return (
      <Container>
        <h1>Upload a smart contract</h1>
        <FileInput></FileInput>
      </Container>
    );
  }
}

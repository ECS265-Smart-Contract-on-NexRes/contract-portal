import React, { Component } from 'react';
import { Button } from 'reactstrap';
export class Home extends Component {
  static displayName = Home.name;

  constructor(props) {
    super(props);
    this.state = { fileNames: [], loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderBinariesTable(fileNames) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>File Name</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {fileNames.map(fileName =>
            <tr>
              <td>{fileName}</td>
              <td>
                <Button color="primary" size="sm">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-cloud-download-fill" viewBox="0 0 16 16">
                    <path fill-rule="evenodd" d="M8 0a5.53 5.53 0 0 0-3.594 1.342c-.766.66-1.321 1.52-1.464 2.383C1.266 4.095 0 5.555 0 7.318 0 9.366 1.708 11 3.781 11H7.5V5.5a.5.5 0 0 1 1 0V11h4.188C14.502 11 16 9.57 16 7.773c0-1.636-1.242-2.969-2.834-3.194C12.923 1.999 10.69 0 8 0zm-.354 15.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 14.293V11h-1v3.293l-2.146-2.147a.5.5 0 0 0-.708.708l3 3z" />
                  </svg>
                </Button>
                {'  '}
                <Button color="danger" size="sm">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-square-fill" viewBox="0 0 16 16">
                    <path d="M2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2zm3.354 4.646L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 1 1 .708-.708z" />
                  </svg>
                </Button></td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Home.renderBinariesTable(this.state.fileNames);

    return (
      <div>
        <h1 id="tabelLabel" >Binaries</h1>
        {contents}
      </div>
    );
  }

  async populateWeatherData() {
    const response = await fetch('api/binary/list');
    const data = await response.json();
    this.setState({ fileNames: data, loading: false });
  }
}

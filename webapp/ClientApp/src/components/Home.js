import React, { Component } from 'react';
import { Button } from 'reactstrap';
export class Home extends Component {
  static displayName = Home.name;

  constructor(props) {
    super(props);
    this.state = { kvStatus: [], loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  renderBinariesTable(kvStatus) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>File Name</th>
            <th>Key</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {kvStatus.map((item, index) =>
            <tr key={index}>
              <td>{item.name}</td>
              <td>{item.key}</td>
              <td>{item.isPublished.toString()}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderBinariesTable(this.state.kvStatus);

    return (
      <div>
        <h1 id="tabelLabel" >Smart Contracts</h1>
        {contents}
      </div>
    );
  }

  async populateWeatherData() {
    const response = await fetch('api/binary/list');
    const data = await response.json();
    this.setState({ kvStatus: data, loading: false });
  }
}

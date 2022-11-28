import React, { Component } from 'react';
import { Button } from 'reactstrap';
import OperationModal from './OperationModal';
export class Home extends Component {
  static displayName = Home.name;

  constructor(props) {
    super(props);
    this.state = { kvStatus: [], loading: true, selectedKey: null };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  undoSelect() {
    this.setState({...this.state, selectedKey: null});
  }

  renderBinariesTable(kvStatus) {
    return (
      <div>
        {kvStatus.map((item) =>
          <OperationModal key={item.key} selectedKey={this.state.selectedKey} undo={this.undoSelect.bind(this)} modal={false} kvStatus={item}></OperationModal>
        )}
        <table className='table table-striped' aria-labelledby="tabelLabel">
          <thead>
            <tr>
              <th>File Name</th>
              <th>Key</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {kvStatus.map((item) =>
              <tr
                onClick={() => {this.setState({...this.state, selectedKey: item.key})}}
                key={item.key}>
                <td>{item.name}</td>
                <td>{item.key}</td>
                <td>{item.isPublished.toString()}</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
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

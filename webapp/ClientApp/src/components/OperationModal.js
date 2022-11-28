
import React from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

class OperationModal extends React.Component {
  constructor(props) {
    super(props);
    this.toggle = this.toggle.bind(this);
    this.isOpen = this.isOpen.bind(this);
  }

  toggle() {
    this.props.undo();
  }

  isOpen() {
    return this.props.selectedKey === this.props.kvStatus.key
  }

  render() {
    return (
      <div>
        <Modal isOpen={this.isOpen()} toggle={this.toggle} className={this.props.className}>
          <ModalHeader toggle={this.toggle}>{this.props.kvStatus.name}</ModalHeader>
          <ModalBody>
            {this.props.kvStatus.content}
          </ModalBody>
          <ModalFooter>
            <Button color="primary" onClick={this.toggle}>Do Something</Button>{' '}
            <Button color="secondary" onClick={this.toggle}>Cancel</Button>
          </ModalFooter>
        </Modal>
      </div>
    );
  }
}

export default OperationModal;
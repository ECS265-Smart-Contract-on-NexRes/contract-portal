
import React from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { CopyBlock } from "react-code-blocks"

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
            <CopyBlock 
              text={this.props.kvStatus.content ? this.props.kvStatus.content : ""}
              language="javascript"
              showLineNumbers={false}
              theme="dracula"
            />
          </ModalBody>
        </Modal>
      </div>
    );
  }
}

export default OperationModal;
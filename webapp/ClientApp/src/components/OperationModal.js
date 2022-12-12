
import React from 'react';
import { Modal, ModalHeader, ModalBody, Container, Row } from 'reactstrap';
import { CopyBlock } from "react-code-blocks"

export default function OperationModal(props) {

  const toggle = function () {
    props.undo();
  }

  const isOpen = function () {
    return props.selectedKey === props.kvStatus.key
  }

  return (
    <Modal size="lg" isOpen={isOpen()} toggle={toggle} className={props.className}>
      <ModalHeader toggle={toggle}>{props.kvStatus.name}</ModalHeader>
      <ModalBody>
        <Container>
          <Row>
            <CopyBlock
              text={props.kvStatus.content ? props.kvStatus.content : ""}
              language="javascript"
              showLineNumbers={false}
              theme="dracula"
            />
          </Row>
        </Container>
      </ModalBody>
    </Modal>
  );
}
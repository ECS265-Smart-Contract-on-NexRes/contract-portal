
import React from 'react';
import { Button, Modal, ModalHeader, ModalBody, Card, CardBody, Spinner, Input, CardFooter, CardHeader, Container, Row, Col, FormGroup } from 'reactstrap';
import { CopyBlock } from "react-code-blocks"

class OperationModal extends React.Component {
  constructor(props) {
    super(props);
    this.toggle = this.toggle.bind(this);
    this.isOpen = this.isOpen.bind(this);
    this.state = { balance: null };
  }

  toggle() {
    this.props.undo();
  }

  isOpen() {
    return this.props.selectedKey === this.props.kvStatus.key
  }

  render() {
    return (

      <Modal size="lg" isOpen={this.isOpen()} toggle={this.toggle} className={this.props.className}>
        <ModalHeader toggle={this.toggle}>{this.props.kvStatus.name}</ModalHeader>
        <ModalBody>
          <Container>
            <Row>
              <Col md="12" lg="8">
                <CopyBlock
                  text={this.props.kvStatus.content ? this.props.kvStatus.content : ""}
                  language="javascript"
                  showLineNumbers={false}
                  theme="dracula"
                />
              </Col>
              <Col md="12" lg="4">
                <Card
                  className="my-2">
                  <CardHeader>
                    User Balance
                  </CardHeader>
                  <CardBody>
                    {this.state.balance == null ? <Spinner color="primary">
                      Loading...
                    </Spinner> : this.state.balance
                    }
                  </CardBody>
                  <CardFooter>
                    <FormGroup>
                      <Input

                      />
                    </FormGroup>

                    <Button type="submit">Add</Button>
                  </CardFooter>
                </Card>
              </Col>
            </Row>
          </Container>
        </ModalBody>
      </Modal>

    );
  }

  componentDidMount() {
    this.GetBalance();
  }

  async GetBalance() {
    const response = await fetch('api/balance/get');
    const data = await response.json();
    this.setState({ balance: data });
  }
}

export default OperationModal;
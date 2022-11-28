
import React from 'react';
import { Button, Modal, ModalHeader, ModalBody, Card, CardBody, Spinner, Input, CardFooter, CardHeader, Container, Row, Col, FormGroup } from 'reactstrap';
import { CopyBlock } from "react-code-blocks"

class OperationModal extends React.Component {
  constructor(props) {
    super(props);
    this.toggle = this.toggle.bind(this);
    this.isOpen = this.isOpen.bind(this);
    this.state = { balance: null, add: 0 };
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
                        onChange={(e) => {
                          this.setState({ ...this.state, add: e.target.value })
                        }}
                      />
                    </FormGroup>
                    <Button onClick={() => {
                      this.updateBalance(this.state.add);}}>Add</Button>
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
    this.getBalance();
  }

  async getBalance() {
    const response = await fetch('api/balance/get');
    const data = await response.json();
    this.setState({ balance: data });
  }

  async updateBalance(add) {
    const response = await fetch(`api/balance/update/${add}`, {
      method: 'POST'
    }).then((res) => {
      if (!res.ok) {
        // make the promise be rejected if we didn't get a 2xx response
        throw new Error("Not 2xx response", { cause: res });
      }
    })
    .then(this.getBalance.bind(this))
    .catch((e) => {
      console.log(`Update balance failed`); 
    });
  }
}

export default OperationModal;

import React, { useEffect, useState } from 'react';
import { Button, Modal, ModalHeader, ModalBody, Card, CardBody, Spinner, Input, CardFooter, CardHeader, Container, Row, Col, FormGroup } from 'reactstrap';
import { CopyBlock } from "react-code-blocks"
import { useFetchWrapper } from "../_helpers/fetchWrapper"

export default function OperationModal(props) {
  const fetchWrapper = useFetchWrapper();
  const [balance, setBalance] = useState(null);
  const [addVal, setAddVal] = useState(0);

  const getBalance = async function () {
    const response = await fetchWrapper.get('api/balance/get');
    const data = await response.json();
    setBalance(data);
  }

  useEffect(() => {
    getBalance();});

  const toggle = function () {
    props.undo();
  }

  const isOpen = function () {
    return props.selectedKey === props.kvStatus.key
  }


  const updateBalance = async function (add) {
    const response = await fetchWrapper.post(`api/balance/update/${add}`)
      .then(getBalance)
      .catch((e) => {
        console.log(`Update balance failed`);
      });
  }

  return (
    <Modal size="lg" isOpen={isOpen()} toggle={toggle} className={props.className}>
      <ModalHeader toggle={toggle}>{props.kvStatus.name}</ModalHeader>
      <ModalBody>
        <Container>
          <Row>
            <Col md="12" lg="8">
              <CopyBlock
                text={props.kvStatus.content ? props.kvStatus.content : ""}
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
                  {balance == null ? <Spinner color="primary">
                    Loading...
                  </Spinner> : balance
                  }
                </CardBody>
                <CardFooter>
                  <FormGroup>
                    <Input
                      onChange={(e) => {
                        setAddVal(e.target.value)
                      }}
                    />
                  </FormGroup>
                  <Button onClick={() => {
                    updateBalance(addVal);
                  }}>Add</Button>
                </CardFooter>
              </Card>
            </Col>
          </Row>
        </Container>
      </ModalBody>
    </Modal>
  );
}
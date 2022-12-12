import React, { Component, useEffect, useState } from 'react';
import { Container, Form, FormGroup, Label, Input, FormText, Button, Alert } from 'reactstrap';
import { useFetchWrapper } from '../_helpers/fetchWrapper';

export const Transfer = function () {
    const [users, setUserIds] = useState([]);
    const [contracts, setContracts] = useState([]);
    const [amount, setAmount] = useState(0);

    const [selectedUserId, setSelectedUserId] = useState(null);
    const [selectedContractId, setSelectedContractId] = useState(null);

    const [showSuccessAlert, setSuccessAlert] = useState(false);
    const [showFailureAlert, setFailureAlert] = useState(false);

    const [failureMsg, setFailureMsg] = useState('');

    const fetchWrapper = useFetchWrapper();

    useEffect(() => {
        fetchWrapper
            .get('api/user/list')
            .then((response) => {
                setUserIds(response);
            });
        fetchWrapper
            .get('api/binary/list')
            .then((response) => {
                setContracts(response);
            });
    }, []);

    const handleSubmit = async (e) => {
        e.preventDefault();

        fetchWrapper.post('api/balance/update', {
            contractId: selectedContractId,
            recipient: selectedUserId,
            valStr: amount
        })
            .then((res) => {
                setSuccessAlert(true);
            }).catch((e) => {
                console.log(`binary upload failed: ${e}`);
                setFailureMsg(e)
                setFailureAlert(true);
            });
    }

    const onSuccessAlertDismiss = () => setSuccessAlert(false);
    const onFailureAlertDismiss = () => setFailureAlert(false);

    return (
        <Container>
            <h1>Transfer</h1>
            <Form >
                <FormGroup>
                    <Label for="recipientSelect">
                        Select a recipient
                    </Label>
                    <Input
                        id="recipientSelect"
                        name="recipient"
                        type="select"
                    >
                        {users.map((usr) =>
                            <option value={`${usr.id}`} key={usr.id}
                                onChange={(e) => setSelectedUserId(e.target.value)}>
                                {`${usr.username}(${usr.id})`}
                            </option>
                        )}
                    </Input>
                </FormGroup>
                <FormGroup>
                    <Label for="contractSelect">
                        Select a smart contract
                    </Label>
                    <Input
                        id="contractSelect"
                        name="contract"
                        type="select"
                    >
                        {contracts.map((contract) =>
                            <option value={`${contract.key}`} key={contract.key}
                                onChange={(e) => setSelectedContractId(e.target.value)}>
                                {`${contract.name}(${contract.key})`}
                            </option>
                        )}
                    </Input>
                </FormGroup>
                <FormGroup>
                    <Label for="amount">
                        Amount
                    </Label>
                    <Input
                        id="amount"
                        name="amount"
                        value={amount}
                        onChange={(e) => setAmount(e.target.value)}
                    />
                </FormGroup>
                <Button color="primary">
                    Submit
                </Button>
            </Form>
            <br></br>
            <Alert color="info" isOpen={showSuccessAlert} toggle={onSuccessAlertDismiss}>
                Transaction succeeded! You can check your current balance and all your previous transactions
            </Alert>
            <Alert color="danger" isOpen={showFailureAlert} toggle={onFailureAlertDismiss}>
                Transaction failed! Reason: {failureMsg}
            </Alert>
        </Container>
    );
}

import React, { useEffect, useReducer, useState } from 'react';
import { Input, Navbar, NavbarBrand, NavItem, NavLink, Nav, TabContent, Container, Row, Col, Card, CardTitle, CardText, Button, List, ListGroupItem, ListGroup, CardBody } from 'reactstrap';
import { Link, Outlet } from 'react-router-dom';
import './NavMenu.css';
import { useRecoilValue } from 'recoil';
import { authAtom } from '../_state';
import { history } from '../_helpers/history';
import { useFetchWrapper } from '../_helpers';
import ProgressButton, { STATE } from 'react-progress-button';
import { useRecoilState } from 'recoil';
import { userBalanceAtom } from '../_state';

export function NavMenu() {
  const [balance]  = useRecoilState(userBalanceAtom);
  const [activeTab, setActiveTab] = useState(window.location.pathname);
  const [privateKey, setPrivateKey] = useState('');
  const [updateButtonState, setUpdateButtonState] = useState('');
  const fetchWrapper = useFetchWrapper();
  const auth = useRecoilValue(authAtom);

  const toggle = function (tab) {
    if (activeTab != tab) {
      setActiveTab(tab);
    }
  }

  const UpdatePrivateKey = function () {
    setUpdateButtonState(STATE.LOADING);
    fetchWrapper.put('api/user/privatekey', { newPrivateKey: privateKey })
      .then(() => {
        setUpdateButtonState(STATE.SUCCESS);
      })
      .catch(() => {
        setUpdateButtonState(STATE.ERROR);
      })
  }

  const getPrivateKey = function () {
    fetchWrapper.get('api/user/privatekey')
      .then((res) => {
        setPrivateKey(res.privateKey);
      });
  }

  useEffect(getPrivateKey, [])

  return (
    <div>
      <header>
        <Navbar className="navbar navbar-expand navbar-dark bg-dark" container light>
          <NavbarBrand tag={Link} to="/">Contract Portal</NavbarBrand>
        </Navbar>
      </header>
      <Container style={{ marginTop: '20px' }}>
        <Row>
          <Col md="12" lg="5">
            <Card
              body
              className="my-2"
            >
              <CardTitle tag="h5">
                Hello <b>{auth.username}</b>
              </CardTitle>
              <CardText>
                You are currently logged in as {auth.username} and you can:
                <List>
                  <li>
                    View all contracts uploaded by you
                  </li>
                  <li>
                    Upload a new contract
                  </li>
                  <li>
                    Make a transaction based on a selected contract uploaded by you
                  </li>
                  <li>
                    View all transactions related to you
                  </li>
                </List>
              </CardText>
              <ListGroup flush>
                <ListGroupItem>
                  <b>Your Balance:</ b> {balance}
                </ListGroupItem>
                <ListGroupItem>
                  <div className='mb-3'>
                    <b>Your private key:</ b>
                  </div>
                  <div className='mb-3'>
                    <Input
                      rows="19"
                      id="exampleText"
                      name="text"
                      type="textarea"
                      value={privateKey}
                      onChange={(e) => {
                        setPrivateKey(e.target.value);
                      }}
                    />
                  </div>
                  <div className='mb-3'>
                    <ProgressButton onClick={UpdatePrivateKey} state={updateButtonState}>
                      Update!
                    </ProgressButton>
                  </div>
                </ListGroupItem>
              </ListGroup>
              <CardBody>
                <div><b>User log out here:</b></div>
                <Button
                  onClick={() => {
                    localStorage.clear();
                    history.push('/');
                    history.go('/');
                  }}
                  color="danger">
                  <b>log out</ b>
                </Button>
              </CardBody>
            </Card>
          </Col>
          <Col md="12" lg="7">
            <Nav tabs>
              <NavItem>
                <NavLink tag={Link}
                  className={activeTab === "/" ? 'active' : ''}
                  to="/"
                  onClick={() => { this.toggle('/'); }}>
                  View contracts
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link}
                  className={activeTab === "/upload" ? 'active' : ''}
                  to="/upload"
                  onClick={() => { this.toggle('/upload'); }}>
                  Upload a contract
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link}
                  className={activeTab === "/transfer" ? 'active' : ''}
                  to="/transfer"
                  onClick={() => { this.toggle('/transfer'); }}>
                  Make a transaction
                </NavLink>
              </NavItem>
            </Nav>
            <TabContent activeTab="1">
            </TabContent>
            <Outlet />
          </Col>
        </Row>
      </Container>
    </div>
  );
}

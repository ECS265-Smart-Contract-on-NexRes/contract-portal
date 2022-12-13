import React, { useEffect, useReducer, useState } from 'react';
import { Alert, Input, Navbar, NavbarBrand, NavItem, NavLink, Nav, TabContent, Container, Row, Col, Card, CardTitle, CardText, Button, List, ListGroupItem, ListGroup, CardBody } from 'reactstrap';
import { Link, Outlet } from 'react-router-dom';
import './NavMenu.css';
import { useRecoilValue } from 'recoil';
import { authAtom } from '../_state';
import { history } from '../_helpers/history';
import { useFetchWrapper } from '../_helpers';
import { RevolvingDot } from 'react-loader-spinner';
import { useRecoilState } from 'recoil';
import { userBalanceAtom } from '../_state';

export function NavMenu() {
  const [balance, setUserBalance] = useRecoilState(userBalanceAtom);
  const [activeTab, setActiveTab] = useState(window.location.pathname);
  const [privateKey, setPrivateKey] = useState('');
  const auth = useRecoilValue(authAtom);
  const [isUploadingPrivateKey, setIsUploadingPrivateKey] = useState(false);

  const fetchWrapper = useFetchWrapper();

  function loadBalance() {
    return fetchWrapper.get('api/balance/get')
      .then((res) => {
        setUserBalance(res);
      });
  }

  const toggle = function (tab) {
    if (activeTab != tab) {
      setActiveTab(tab);
    }
  }

  const UpdatePrivateKey = function () {
    setIsUploadingPrivateKey(true);
    fetchWrapper.put('api/user/privatekey', { newPrivateKey: privateKey })
      .then(() => {
        setIsUploadingPrivateKey(false);
      })
      .then(loadBalance)
      .catch((e) => {
        setIsUploadingPrivateKey(false);
        setUserBalance(null);
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
                  <b>Your Balance:</ b> {balance == null ?
                    <Alert color="danger">
                      The website need your <b>correct</ b> private key to query you balance.
                    </Alert> :
                    balance}
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
                  <container className='mb-3'>
                    <Row>
                      <Col xs="3">
                        <Button
                          onClick={UpdatePrivateKey}
                          color="primary">
                          <b>Upload</ b>
                        </Button>
                      </Col>
                      <Col xs="3">
                        {isUploadingPrivateKey ? <RevolvingDot
                          height="30"
                          width="30"
                          color="#4fa94d"
                          secondaryColor=''
                          ariaLabel="revolving-dot-loading"
                          radius="5"
                          wrapperStyle={{}}
                          wrapperClass=""
                          visible={true}
                        /> : null}
                      </Col>
                    </Row>

                  </container>
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
                  <b>Log out</ b>
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

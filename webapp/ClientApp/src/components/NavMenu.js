import React, { useEffect, useState } from 'react';
import { Navbar, NavbarBrand, NavItem, NavLink, Nav, TabContent, Container, Row, Col, Card, CardTitle, CardText, Button, List, ListGroupItem, ListGroup, CardBody } from 'reactstrap';
import { Link, Outlet } from 'react-router-dom';
import './NavMenu.css';
import { useRecoilValue } from 'recoil';
import { authAtom } from '../_state';
import { history } from '../_helpers/history';
import { useFetchWrapper } from '../_helpers';

export function NavMenu() {
  const [activeTab, setActiveTab] = useState(window.location.pathname);
  const [balance, setBalance] = useState(null);
  const fetchWrapper = useFetchWrapper();
  const auth = useRecoilValue(authAtom);

  const toggle = function (tab) {
    if (activeTab != tab) {
      setActiveTab(tab);
    }
  }

  const getBalance = function () {
    fetchWrapper.get('api/balance/get')
      .then((res) => { 
        setBalance(res);
      })
  }

  useEffect(getBalance, [])

  return (
    <div>
      <header>
        <Navbar className="navbar navbar-expand navbar-dark bg-dark" container light>
          <NavbarBrand tag={Link} to="/">Contract Portal</NavbarBrand>
        </Navbar>
      </header>
      <Container style={{ marginTop: '20px' }}>
        <Row>
          <Col md="12" lg="3">
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
              </ListGroup>
              <CardBody>
                <Button
                  onClick={() => {
                    localStorage.clear();
                    history.push('/');
                    history.go('/');
                  }}
                  color="danger">
                  Log out
                </Button>
              </CardBody>
            </Card>
          </Col>
          <Col md="12" lg="9">
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

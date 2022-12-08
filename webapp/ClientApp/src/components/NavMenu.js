import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, Nav, TabContent, TabPane, Row, Col, Card, CardText, CardTitle, Button, Container } from 'reactstrap';
import { Link, Outlet } from 'react-router-dom';
import './NavMenu.css';
import {classnames} from 'classnames';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true,
      activeTab: '1'
    };
    this.toggle = this.toggle.bind(this);
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  toggle(tab) {
    if (this.state.activeTab !== tab) {
      this.setState({ activeTab: tab });
    }
  }

  render() {
    return (
      <div>
        <header>
          <Navbar className="navbar navbar-expand navbar-dark bg-dark" container light>
            <NavbarBrand tag={Link} to="/">Contract Portal</NavbarBrand>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow">
                <NavItem>
                  
                </NavItem>
              </ul>
            </Collapse>
          </Navbar>
        </header>
        <Container>
          <Nav tabs>
            <NavItem>
              <NavLink tag={Link} 
                className={this.state.activeTab === '1' ? 'active' : ''}
                to="/" 
                onClick={() => { this.toggle('1'); }}>
                View
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink tag={Link} 
                className={this.state.activeTab === '2' ? 'active' : ''}
                to="/upload" 
                onClick={() => { this.toggle('2'); }}>
                Upload
              </NavLink>
            </NavItem>
          </Nav>
          <TabContent activeTab="1">
          </TabContent>
        </Container>
        <Outlet />
      </div>
    );
  }
}

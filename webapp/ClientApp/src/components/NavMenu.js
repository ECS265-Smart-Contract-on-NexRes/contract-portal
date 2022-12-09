import React, { useEffect, useState } from 'react';
import { Navbar, NavbarBrand, NavItem, NavLink, Nav, TabContent, Container } from 'reactstrap';
import { Link, Outlet } from 'react-router-dom';
import './NavMenu.css';

export function NavMenu() {
  const [activeTab, setActiveTab] = useState(window.location.pathname);

  const toggle = function (tab) {
    if (activeTab != tab) {
      setActiveTab(tab);
    }
  }

  return (
    <div>
      <header>
        <Navbar className="navbar navbar-expand navbar-dark bg-dark" container light>
          <NavbarBrand tag={Link} to="/">Contract Portal</NavbarBrand>
        </Navbar>
      </header>
      <Container>
        <Nav tabs>
          <NavItem>
            <NavLink tag={Link}
              className={activeTab === "/" ? 'active' : ''}
              to="/"
              onClick={() => { this.toggle('/'); }}>
              View
            </NavLink>
          </NavItem>
          <NavItem>
            <NavLink tag={Link}
              className={activeTab === "/upload" ? 'active' : ''}
              to="/upload"
              onClick={() => { this.toggle('/upload'); }}>
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

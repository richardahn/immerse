import React, { Component } from 'react';
import { Route } from 'react-router';
import { AppLayout } from './components/AppLayout';
import { Home } from './components/Home';
import 'antd/dist/antd.css';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <AppLayout>
        <Route exact path='/' component={Home} />
      </AppLayout>
    );
  }
}

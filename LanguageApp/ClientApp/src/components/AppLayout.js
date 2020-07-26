import React, { Component } from 'react';
import { Layout, Select, Spin, Button, Typography } from 'antd';

const { Option } = Select;
const { Header, Content } = Layout;
const { Text, Title } = Typography;

export class AppLayout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <Layout style={{ backgroundColor: 'white' }}>
        <Header style={{ backgroundColor: 'white', boxShadow: '0 0 16px gray' }}>
          <Text style={{ fontSize: '1.5rem' }}>Quiz App</Text>
        </Header>
        {this.props.children}
      </Layout>
    );
  }
}

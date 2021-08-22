import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Header from './header.jsx';
import GameList from './gamelist.jsx';
import MainPage from './main.jsx';
import Game from './game.jsx';
import Login from './login.jsx'
import Register from './register.jsx'
import Profile from './profile.jsx'

export default class App extends React.Component {
    render() {
        return (
            <div>
                <Header />
                <div className="container-fluid">
                    <main role="main" className="pb-3">
                        <Route path='/' exact component={MainPage} />
                        <Route path='/Account/Register' component={Register} />
                        <Route path='/Account/Login' component={Login} />
                        <Route path='/GameList' component={GameList} />
                        <Route path='/Game/:gameId' component={Game} />
                        <Route path='/Account/Profile/:userName?' component={Profile} />
                    </main>
                </div>
            </div>
        );
    }
};
﻿import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Header from './header.jsx';
import GameList from './gamelist.jsx';
import MainPage from './main.jsx';
import Game from './game.jsx';

export default class App extends React.Component {
    render() {
        return (
            <div>
                <Header />
                <div className="container-fluid">
                    <main role="main" className="pb-3">
                        <Route path='/' exact component={MainPage} />
                        <Route path='/GameList' component={GameList} />
                        <Route path='/Game/:gameId' component={Game} />
                    </main>
                </div>
            </div>
        );
    }
};
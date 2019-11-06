import React, { Component } from 'react';
import { Switch, Route } from 'react-router-dom';
import TicketComponent from "../components/TicketComponent";
import EditTicketComponent from "../components/EditTicketComponent";
import DeleteTicketComponent from "../components/DeleteTicketComponent";

class RouterRoutes extends Component {
    constructor(props) {
        super(props);

        this.state = ({
            parent: props.parent
        });
    }

    render() {
        return <Switch>
            <Route exact path='/' render={(props) => <TicketComponent {...props} parent={this.state.parent}/>}/>
            <Route exact path='/edit' render={(props) => <EditTicketComponent {...props} parent={this.state.parent}/>}/>
            <Route exact path='/delete' render={(props) => <DeleteTicketComponent {...props} parent={this.state.parent}/>}/>
        </Switch>
    }
}

export default RouterRoutes;
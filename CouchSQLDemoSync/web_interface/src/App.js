import React, { Component } from 'react';
import PouchDB from 'pouchdb';

import Center from 'react-center';
import { Sidebar, Segment, Input, Card, Button, Dropdown } from 'semantic-ui-react';

import {withRouter} from 'react-router-dom';

import './App.css';
import RouterRoutes from "./router/RouterRoutes";

class App extends Component {
    constructor(props) {
        super(props);

        this.state = ({
            visible: false,
            page: 'add',

            connection_string: "http://localhost:12345/api/Tickets",
            sync: "Tickets",

            // new ticket
            ActivityCount: 1,
            City: "Salzburg",
            Country: "A",
            Description: "Kurzbeschreibung-Text",
            EndDate: "28.02.2018 08:00:00",
            IsFixedTimeFrame: false,
            LatestCompletionDate: "",
            LocationAddress1: "StraßenName Hausnummer",
            LocationAddress2: "",
            LocationName1: "Firmenname",
            LocationName2: "",
            ServiceCallId: "Firma/TicketID",
            ServiceCallLineItemCount: 1,
            StartDate: "28.02.2018 08:00:00",
            PersId: "PersonenIdentifizierung",
            Title: "Firma",
            ZipCode: "5412",
            TicketId: "2468"
        });

        this.dbTicket = new PouchDB('sync_v1_ticket');

        this.props.history.push('/');

        this.handleChange = this.handleChange.bind(this);
    }

    useRnsUseCaseDatabase() {
        this.setState({connection_string: "http://localhost:5984/rns_use_case"})
    }

    getRemote() {
        this.remote = new PouchDB(this.state.connection_string);
    }

    getRemoteInfo() {
        if (this.remote !== null && this.remote !== undefined) {
            this.remote.info().then(function (info) {
                console.log(info);
            })
        }
        else {
            alert('No Target Database');
        }
    }

    startSync() {
        if (this.remote !== null && this.remote !== undefined) {
            if (this.state.sync === "Tickets") {
                this.dbTicket.sync(this.remote
                ).on('change', function (change) {
                    console.log("yo, something changed!");
                }).on('paused', function (info) {
                    console.log("replication was paused, usually because of a lost connection");
                }).on('active', function (info) {
                    console.log("replication was resumed");
                    console.log(info);
                }).on('error', function (err) {
                    console.log("totally unhandled error (shouldn't happen)");
                    console.log(err);
                });
            }
            else if (this.state.sync === "TicketTests") {
                this.dbTicketTest.sync(this.remote
                ).on('change', function (change) {
                    console.log("yo, something changed!");
                }).on('paused', function (info) {
                    console.log("replication was paused, usually because of a lost connection");
                }).on('active', function (info) {
                    console.log("replication was resumed");
                }).on('error', function (err) {
                    console.log("totally unhandled error (shouldn't happen)");
                });
            }
        }
        else {
            alert('No Target Database');
        }
    }

    startSyncLive() {
        if (this.remote !== null && this.remote !== undefined) {
            if (this.state.sync === "Tickets") {
                this.dbTicket.sync(this.remote, {
                    live: true,
                    retry: true
                }).on('change', function (change) {
                    console.log("yo, something changed!");
                }).on('paused', function (info) {
                    console.log("replication was paused, usually because of a lost connection");
                }).on('active', function (info) {
                    console.log("replication was resumed");
                }).on('error', function (err) {
                    console.log("totally unhandled error (shouldn't happen)");
                });
            }
            else if (this.state.sync === "TicketTests") {
                this.dbTicketTest.sync(this.remote, {
                    live: true,
                    retry: true
                }).on('change', function (change) {
                    console.log("yo, something changed!");
                }).on('paused', function (info) {
                    console.log("replication was paused, usually because of a lost connection");
                }).on('active', function (info) {
                    console.log("replication was resumed");
                }).on('error', function (err) {
                    console.log("totally unhandled error (shouldn't happen)");
                });
            }
        }
        else {
            alert('No Target Database');
        }
    }

    submitTestTicket() {
        let Ticket = {
            ticket_id: this.state.ticket_id,
            customer: this.state.customer,
            date: this.state.date,
            work_time: this.state.work_time
        };

        this.dbTicketTest.post(Ticket)
            .then(function (response) {
                console.log('successfully posted into client dbTicketTest: ' + JSON.stringify(response));
            }).catch(function (err) {
            console.log(err);
        });
    }

    submitTicket() {
        let Ticket = {
            ActivityCount: this.state.ActivityCount,
            City: this.state.City,
            Country: this.state.Country,
            Description: this.state.Description,
            EndDate: this.state.End,
            IsFixedTimeFrame: this.state.IsFixedTimeFrame,
            LatestCompletionDate: this.state.LatestCompletionDate,
            LocationAddress1: this.state.LocationAddress1,
            LocationAddress2: this.state.LocationAddress2,
            LocationName1: this.state.LocationName1,
            LocationName2: this.state.LocationName2,
            ServiceCallId: this.state.ServiceCallId,
            ServiceCallLineItemCount: this.state.ServiceCallLineItemCount,
            StartDate: this.state.StartDate,
            PersId: this.state.PersId,
            Title: this.state.Title,
            ZipCode: this.state.ZipCode,
            TicketId: this.state.TicketId
        };

        this.dbTicket.post(Ticket)
            .then(function (response) {
                console.log('successfully posted into client dbTicket: ' + JSON.stringify(response));
            }).catch(function (err) {
            console.log(err);
        });
    }

    getDevViewSidebar() {
        if (this.state.visible){
            return <Sidebar as={Segment} animation="scale down" direction="top" visible={this.state.visible}>
                <h3>Developer Options</h3>
                <div className="Margin-Bottom-8px">
                    <div>Connection String</div>
                    <Input name="connection_string" className="ChildElem Input-MinWidth-300" value={this.state.connection_string} onChange={this.handleChange}/>
                </div>
                <div className="Margin-Bottom-8px">
                    <div>DB to Sync</div>
                    <Input name="sync" className="ChildElem Input-MinWidth-300" value={this.state.sync} onChange={this.handleChange}/>
                </div>
                <div>Sync</div>
                <div className="ChildElem">
                    <Button content="Connect DB" onClick={() => this.getRemote()}/>
                    <Button content="Get DB Info" onClick={() => this.getRemoteInfo()}/>
                    <Button content="Start Sync" onClick={() => this.startSync()}/>
                    <Button content="Start Sync Live" onClick={() => this.startSyncLive()}/>
                    <Button content="Use old UseCaseDB" onClick={() => this.useRnsUseCaseDatabase()}/>
                </div>
            </Sidebar>
        }
    }

    getDevViewEnablerSidebar() {
        if (this.state.visible){
            return <Sidebar as={Segment} animation="overlay" direction="right" visible={true}>
                <div className="Text-Center Onhover-Pointer Onhover-Fat Padding-4" onClick={() => this.toggleVisibility()}>Click to disable Dev View</div>

                <Center>
                    <span>current Page: </span>
                    <Dropdown text={this.state.page} className="Text-Center Padding-4">
                        <Dropdown.Menu>
                            <Dropdown.Item text='add' onClick={() => this.setPage('add')}/>
                            <Dropdown.Item text='edit' onClick={() => this.setPage('edit')}/>
                            <Dropdown.Item text='delete' onClick={() => this.setPage('delete')}/>
                        </Dropdown.Menu>
                    </Dropdown>
                </Center>
            </Sidebar>
        }
        return <Sidebar as={Segment} animation="overlay" direction="right" visible={true}>
            <div className="Text-Center Onhover-Pointer Onhover-Fat Padding-4" onClick={() => this.toggleVisibility()}>Click to enable Dev View</div>

            <Center>
                <span>current Page: </span>
                <Dropdown text={this.state.page} className="Text-Center Padding-4">
                    <Dropdown.Menu>
                        <Dropdown.Item text='add' onClick={() => this.setPage('add')}/>
                        <Dropdown.Item text='edit' onClick={() => this.setPage('edit')}/>
                        <Dropdown.Item text='delete' onClick={() => this.setPage('delete')}/>
                    </Dropdown.Menu>
                </Dropdown>
            </Center>
        </Sidebar>
    }

    toggleVisibility() {
        this.setState({visible: !this.state.visible});
    }

    handleChange(e) {
        this.setState({
            [e.target.name]: e.target.value
        });
    }

    setPage(page) {
        this.setState({page: page});

        if (page === 'add') {
            this.props.history.push('/');
        }
        else {
            this.props.history.push('/' + page);
        }
    }

    render() {
        return (
            <div className="App">
                {this.getDevViewSidebar()}

                <Center className="App">
                    <Card.Group itemsPerRow={1}>
                        <RouterRoutes parent={this}/>
                    </Card.Group>
                </Center>

                {this.getDevViewEnablerSidebar()}
            </div>
        );
    }
}

export default withRouter(App);

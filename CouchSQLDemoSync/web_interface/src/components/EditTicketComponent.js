import React, { Component } from 'react';

import Center from 'react-center';
import { Card, Dropdown, Input, Button } from 'semantic-ui-react';

import './../App.css';

class EditTicketComponent extends Component {
    constructor(props) {
        super(props);

        this.state = ({
            parent: props.parent,

            tickets: [],
            currentTicket: null,
            chosenId: ''
        });

        this.getAllData();

        this.handleChange = this.handleChange.bind(this);
    }

    getAllData() {
        this.state.parent.dbTicket.allDocs({
            include_docs: true,
            attachments: true
        }).then(result => {
            this.setState({tickets: result.rows});
        }).catch(err => {
            console.log(err);
        });
    }

    setChosenId(id) {
        this.setState({chosenId: id});
    }

    componentDidUpdate() {
        this.state.tickets.forEach(item => {
            if (this.state.chosenId === item.id && this.state.currentTicket !== item) {
                this.setState({currentTicket: item});
            }
        })
    }

    update() {
        this.state.parent.dbTicket.get(this.state.chosenId)
            .then(doc => {
                console.log(doc);
                console.log(this.state.currentTicket);
                this.state.parent.dbTicket.put(this.state.currentTicket.doc);
            });
    }

    handleChange(e) {
        let newTicket = this.state.currentTicket;
        newTicket['doc'][e.target.name] = e.target.value;

        this.setState({
            currentTicket: newTicket
        });
    }

    renderTicket() {
        if (this.state.currentTicket !== null) {
            return <div>
                <br/>
                <div className="Margin-8">
                    <strong className='Text-Left'>Id:</strong>
                    <span className='Text-Right'>{this.state.currentTicket.id}</span>
                </div>
                <div className="Margin-8">
                    <strong className='Text-Left'>Key:</strong>
                    <span className='Text-Right'>{this.state.currentTicket.key}</span>
                </div>
                <div className="Margin-8">
                    <strong className='Text-Left'>Rev:</strong>
                    <span className='Text-Right'>{this.state.currentTicket.value.rev}</span>
                </div>
                <br/>

                <Center><Input name="ActivityCount" className="Margin-8" placeholder="Activity Count" onChange={this.handleChange} value={this.state.currentTicket.doc.ActivityCount}/></Center>
                <Center><Input name="City" className="Margin-8" placeholder="City" onChange={this.handleChange} value={this.state.currentTicket.doc.City}/></Center>
                <Center><Input name="Country" className="Margin-8" placeholder="Country" onChange={this.handleChange} value={this.state.currentTicket.doc.Country}/></Center>
                <Center><Input name="Description" className="Margin-8" placeholder="Description" onChange={this.handleChange} value={this.state.currentTicket.doc.Description}/></Center>
                <Center><Input name="EndDate" className="Margin-8" placeholder="End Date" onChange={this.handleChange} value={this.state.currentTicket.doc.EndDate}/></Center>
                <Center><Input name="IsFixedTimeFrame" className="Margin-8" placeholder="Is Fixed Time Frame" onChange={this.handleChange} value={this.state.currentTicket.doc.IsFixedTimeFrame}/></Center>
                <Center><Input name="LatestCompletionDate" className="Margin-8" placeholder="Latest Completion Date" onChange={this.handleChange} value={this.state.currentTicket.doc.LatestCompletionDate}/></Center>
                <Center><Input name="LocationAddress1" className="Margin-8" placeholder="Location Address 1" onChange={this.handleChange} value={this.state.currentTicket.doc.LocationAddress1}/></Center>
                <Center><Input name="LocationAddress2" className="Margin-8" placeholder="Location Address 2" onChange={this.handleChange} value={this.state.currentTicket.doc.LocationAddress2}/></Center>
                <Center><Input name="LocationName1" className="Margin-8" placeholder="Location Name 1" onChange={this.handleChange} value={this.state.currentTicket.doc.LocationName1}/></Center>
                <Center><Input name="LocationName2" className="Margin-8" placeholder="Location Name 2" onChange={this.handleChange} value={this.state.currentTicket.doc.LocationName2}/></Center>
                <Center><Input name="ServiceCallId" className="Margin-8" placeholder="Service Call Id" onChange={this.handleChange} value={this.state.currentTicket.doc.ServiceCallId}/></Center>
                <Center><Input name="ServiceCallLineItemCount" className="Margin-8" placeholder="Service Call Line Item Count" onChange={this.handleChange} value={this.state.currentTicket.doc.ServiceCallLineItemCount}/></Center>
                <Center><Input name="StartDate" className="Margin-8" placeholder="Start Date" onChange={this.handleChange} value={this.state.currentTicket.doc.StartDate}/></Center>
                <Center><Input name="PersId" className="Margin-8" placeholder="Pers Id" onChange={this.handleChange} value={this.state.currentTicket.doc.PersId}/></Center>
                <Center><Input name="Title" className="Margin-8" placeholder="Title" onChange={this.handleChange} value={this.state.currentTicket.doc.Title}/></Center>
                <Center><Input name="ZipCode" className="Margin-8" placeholder="Zipcode" onChange={this.handleChange} value={this.state.currentTicket.doc.ZipCode}/></Center>
                <Center><Input name="TicketId" className="Margin-8" placeholder="Ticket Id" onChange={this.handleChange} value={this.state.currentTicket.doc.TicketId}/></Center>
            </div>;
        }
        else {
            return null;
        }
    }

    render() {
        return (
            <Card>
                <Card.Content>
                    <h4>Edit Ticket</h4>

                    <Dropdown text='Id'>
                        <Dropdown.Menu>
                            {this.state.tickets.map((ticket,i) => {
                                return <Dropdown.Item key={'del' + i} text={JSON.stringify(ticket.id)} onClick={() => this.setChosenId(ticket.id)}/>
                            })}
                        </Dropdown.Menu>
                    </Dropdown>
                    <br/>
                    {this.renderTicket()}
                </Card.Content>
                <Card.Content extra>
                    <Button className='Text-Right' onClick={() => this.update()}>Commit edit</Button>
                </Card.Content>
            </Card>
        );
    }
}

export default EditTicketComponent;
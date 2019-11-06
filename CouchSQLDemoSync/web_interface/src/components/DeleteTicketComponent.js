import React, { Component } from 'react';

import { Card, Dropdown, Button } from 'semantic-ui-react';
import './../App.css';

class DeleteTicketComponent extends Component {
    constructor(props) {
        super(props);

        this.state = ({
            parent: props.parent,

            tickets: [],
            currentTicket: null,
            chosenId: ''
        });

        this.getAllData();
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

    delete() {
        this.state.parent.dbTicket.get(this.state.chosenId).then(doc => {
            this.state.parent.dbTicket.remove(doc._id, doc._rev).then(() => {
                this.setState({
                    currentTicket: null,
                    chosenId: ''
                });

                this.getAllData();
            })
        });
    }

    renderTicket() {
        if (this.state.currentTicket !== null) {
            return <div>
                <div className="Margin-8"><strong className='Text-Left'>Id:</strong> <span className='Text-Right'>{this.state.currentTicket.id}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Key:</strong> <span className='Text-Right'>{this.state.currentTicket.key}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Rev:</strong> <span className='Text-Right'>{this.state.currentTicket.value.rev}</span></div>
                <br/>
                <div className="Margin-8"><strong className='Text-Left'>Activity Count:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.ActivityCount}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>City:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.City}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Country:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.Country}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Description:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.Description}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>End Date:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.EndDate}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Is fixed timeframe:</strong> <span className='Text-Right'>{JSON.stringify(this.state.currentTicket.doc.IsFixedTimeFrame)}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Latest Completion Date:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.LatestCompletionDate}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Location Address 1:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.LocationAddress1}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Location Address 2:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.LocationAddress2}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Location Name 1:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.LocationName1}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Location Name 2:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.LocationName2}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Service Call Id:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.ServiceCallId}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Service Call Line Item Count:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.ServiceCallLineItemCount}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Start Date:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.StartDate}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Pers Id:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.PersId}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Title:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.Title}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Zip Code:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.ZipCode}</span></div>
                <div className="Margin-8"><strong className='Text-Left'>Ticket Id:</strong> <span className='Text-Right'>{this.state.currentTicket.doc.TicketId}</span></div>
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
                    <h4>Are you sure you want to delete this ticket?</h4>

                    <Dropdown text='Id'>
                        <Dropdown.Menu>
                            {this.state.tickets.map((ticket,i) => {
                                return <Dropdown.Item key={'del' + i} text={JSON.stringify(ticket.id)} onClick={() => this.setChosenId(ticket.id)}/>
                            })}
                        </Dropdown.Menu>
                    </Dropdown>
                    {this.renderTicket()}
                </Card.Content>
                <Card.Content extra>
                    <Button className='Text-Right' onClick={() => this.delete()}>Yes, delete</Button>
                </Card.Content>
            </Card>
        );
    }
}

export default DeleteTicketComponent;

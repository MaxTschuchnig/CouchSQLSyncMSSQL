import React, { Component } from 'react';

import Center from 'react-center';
import { Input, Card, Button } from 'semantic-ui-react';

import './../App.css';

class TicketComponent extends Component {
    constructor(props) {
        super(props);

        this.state = ({
            parent: props.parent
        });
    }

    render() {
        return (
            <Card>
                <Card.Content>
                    <h4>Add new</h4>
                </Card.Content>
                <Card.Content extra>
                    <Center><Input name="ActivityCount" className="Margin-8" placeholder="Activity Count" onChange={this.state.parent.handleChange} value={this.state.parent.state.ActivityCount}/></Center>
                    <Center><Input name="City" className="Margin-8" placeholder="City" onChange={this.state.parent.handleChange} value={this.state.parent.state.City}/></Center>
                    <Center><Input name="Country" className="Margin-8" placeholder="Country" onChange={this.state.parent.handleChange} value={this.state.parent.state.Country}/></Center>
                    <Center><Input name="Description" className="Margin-8" placeholder="Description" onChange={this.state.parent.handleChange} value={this.state.parent.state.Description}/></Center>
                    <Center><Input name="EndDate" className="Margin-8" placeholder="End Date" onChange={this.state.parent.handleChange} value={this.state.parent.state.EndDate}/></Center>
                    <Center><Input name="IsFixedTimeFrame" className="Margin-8" placeholder="Is Fixed Time Frame" onChange={this.state.parent.handleChange} value={this.state.parent.state.IsFixedTimeFrame}/></Center>
                    <Center><Input name="LatestCompletionDate" className="Margin-8" placeholder="Latest Completion Date" onChange={this.state.parent.handleChange} value={this.state.parent.state.LatestCompletionDate}/></Center>
                    <Center><Input name="LocationAddress1" className="Margin-8" placeholder="Location Address 1" onChange={this.state.parent.handleChange} value={this.state.parent.state.LocationAddress1}/></Center>
                    <Center><Input name="LocationAddress2" className="Margin-8" placeholder="Location Address 2" onChange={this.state.parent.handleChange} value={this.state.parent.state.LocationAddress2}/></Center>
                    <Center><Input name="LocationName1" className="Margin-8" placeholder="Location Name 1" onChange={this.state.parent.handleChange} value={this.state.parent.state.LocationName1}/></Center>
                    <Center><Input name="LocationName2" className="Margin-8" placeholder="Location Name 2" onChange={this.state.parent.handleChange} value={this.state.parent.state.LocationName2}/></Center>
                    <Center><Input name="ServiceCallId" className="Margin-8" placeholder="Service Call Id" onChange={this.state.parent.handleChange} value={this.state.parent.state.ServiceCallId}/></Center>
                    <Center><Input name="ServiceCallLineItemCount" className="Margin-8" placeholder="Service Call Line Item Count" onChange={this.state.parent.handleChange} value={this.state.parent.state.ServiceCallLineItemCount}/></Center>
                    <Center><Input name="StartDate" className="Margin-8" placeholder="Start Date" onChange={this.state.parent.handleChange} value={this.state.parent.state.StartDate}/></Center>
                    <Center><Input name="PersId" className="Margin-8" placeholder="Pers Id" onChange={this.state.parent.handleChange} value={this.state.parent.state.PersId}/></Center>
                    <Center><Input name="Title" className="Margin-8" placeholder="Title" onChange={this.state.parent.handleChange} value={this.state.parent.state.Title}/></Center>
                    <Center><Input name="ZipCode" className="Margin-8" placeholder="Zipcode" onChange={this.state.parent.handleChange} value={this.state.parent.state.ZipCode}/></Center>
                    <Center><Input name="TicketId" className="Margin-8" placeholder="Ticket Id" onChange={this.state.parent.handleChange} value={this.state.parent.state.TicketId}/></Center>
                    <br/>
                    <Button className='Text-Right' content="Submit" onClick={() => this.state.parent.submitTicket()}/>
                </Card.Content>
            </Card>
        );
    }
}

export default TicketComponent;

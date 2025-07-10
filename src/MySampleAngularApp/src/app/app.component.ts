import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http'; // Required for HTTP requests
import { CommonModule } from '@angular/common'; // NEW: Required for common directives like *ngIf, *ngFor
import { FormsModule } from '@angular/forms'; // NEW: Required for ngModel

interface Item {
  id: number;
  name: string;
  description: string;
  createdDate: string; // ISO 8601 string
}

@Component({
  selector: 'app-root', // The HTML tag for this component
  templateUrl: './app.component.html', // The HTML template file
  styleUrls: ['./app.component.css'], // The CSS style file
  standalone: true, // NEW: Mark AppComponent as standalone
  imports: [
    CommonModule, // NEW: Import CommonModule for directives
    FormsModule, // NEW: Import FormsModule for ngModel
    // HttpClientModule // REMOVED: HttpClientModule should be imported in AppModule, not here
  ]
})
export class AppComponent implements OnInit {
  title = 'MySampleAngularApp';
  apiHelloMessage: string = ''; // Stores the message from the API's /hello endpoint
  items: Item[] = []; // Stores items fetched from the database
  newItemName: string = ''; // For the new item input field
  newItemDescription: string = ''; // For the new item description input field
  serviceBusMessageContent: string = ''; // For the Service Bus message input field
  serviceBusResponseMessage: string = ''; // Response after sending Service Bus message

  // Inject HttpClient in the constructor
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getHelloMessage(); // Call the hello endpoint on component initialization
    this.getItems(); // Fetch items on component initialization
  }

  // Calls the /api/proxy/hello endpoint on the ASP.NET Core host
  getHelloMessage(): void {
    this.http.get('/sampleapp/api/proxy/hello', { responseType: 'text' }).subscribe({
      next: (data) => {
        this.apiHelloMessage = data;
        console.log('API Hello Message:', data);
      },
      error: (error) => {
        console.error('Error fetching hello message:', error);
        this.apiHelloMessage = 'Error fetching message from API.';
      }
    });
  }

  // Calls the /api/proxy/hello/items endpoint to get all items
  getItems(): void {
    this.http.get<Item[]>('/sampleapp/api/proxy/hello/items').subscribe({
      next: (data) => {
        this.items = data;
        console.log('Fetched Items:', data);
      },
      error: (error) => {
        console.error('Error fetching items:', error);
      }
    });
  }

  // Calls the /api/proxy/hello/items endpoint to add a new item
  addItem(): void {
    if (!this.newItemName) {
      alert('Item name cannot be empty.'); // Using alert for simplicity, consider a better UI notification
      return;
    }

    const newItem: Omit<Item, 'id' | 'createdDate'> = {
      name: this.newItemName,
      description: this.newItemDescription
    };

    this.http.post<Item>('/sampleapp/api/proxy/hello/items', newItem).subscribe({
      next: (addedItem) => {
        console.log('Item added:', addedItem);
        this.items.push(addedItem); // Add the new item to the local list
        this.newItemName = ''; // Clear the input field
        this.newItemDescription = ''; // Clear the description field
        this.getItems(); // Refresh the list to ensure consistency (optional, but good for real-time updates)
      },
      error: (error) => {
        console.error('Error adding item:', error);
        alert('Failed to add item. Check console for details.');
      }
    });
  }

  // Calls the /api/proxy/hello/send-message endpoint to send a Service Bus message
  sendServiceBusMessage(): void {
    if (!this.serviceBusMessageContent) {
      alert('Message content cannot be empty.'); // Using alert for simplicity
      return;
    }

    // The API expects a raw string in the body, so we send it directly
    this.http.post('/sampleapp/api/proxy/hello/send-message', `"${this.serviceBusMessageContent}"`, {
      headers: { 'Content-Type': 'application/json' }, // Explicitly set content type for raw string body
      responseType: 'text' // Expect a plain text response
    }).subscribe({
      next: (response) => {
        this.serviceBusResponseMessage = response;
        console.log('Service Bus Message Sent:', response);
        this.serviceBusMessageContent = ''; // Clear the input field
      },
      error: (error) => {
        console.error('Error sending Service Bus message:', error);
        this.serviceBusResponseMessage = `Error: ${error.message || 'Failed to send message.'}`;
        alert('Failed to send Service Bus message. Check console for details.');
      }
    });
  }
}
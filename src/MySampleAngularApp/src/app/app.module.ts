import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http'; // Required for making HTTP requests
import { FormsModule } from '@angular/forms'; // Required for ngModel (two-way data binding)

import { AppComponent } from './app.component'; // Import AppComponent

@NgModule({
  declarations: [
    // AppComponent // REMOVED: Standalone components are imported, not declared
  ],
  imports: [
    BrowserModule,
    HttpClientModule, // Add HttpClientModule to imports
    FormsModule, // Add FormsModule to imports
    AppComponent // NEW: Import AppComponent as it is now standalone
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
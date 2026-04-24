import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  welcomeMessage = 'Discover Your Next Awesome Event';
  events = [
    { title: 'Neon Nights Concert', imageUrl: 'https://images.unsplash.com/photo-1459749411175-04bf5292ceea?auto=format&fit=crop&w=600&q=80', date: 'Oct 15, 2026' },
    { title: 'Tech Innovators Summit', imageUrl: 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?auto=format&fit=crop&w=600&q=80', date: 'Nov 02, 2026' },
    { title: 'Food & Wine Festival', imageUrl: 'https://images.unsplash.com/photo-1414235077428-33898ed1e828?auto=format&fit=crop&w=600&q=80', date: 'Sep 28, 2026' },
    { title: 'Art Walk Downtown', imageUrl: 'https://images.unsplash.com/photo-1460661419201-fd4cecdf8a8b?auto=format&fit=crop&w=600&q=80', date: 'Oct 05, 2026' },
    { title: 'Startup Pitch Night', imageUrl: 'https://images.unsplash.com/photo-1515187029135-18ee286d815b?auto=format&fit=crop&w=600&q=80', date: 'Dec 10, 2026' },
    { title: 'Local Farmers Market', imageUrl: 'https://images.unsplash.com/photo-1533174000255-bf9754f0a2ca?auto=format&fit=crop&w=600&q=80', date: 'Weekly' },
    { title: 'Symphony in the Park', imageUrl: 'https://images.unsplash.com/photo-1516450360452-9312f5e86fc7?auto=format&fit=crop&w=600&q=80', date: 'Aug 30, 2026' }
  ];
}

<script>
  import { Router, Route, Link } from "svelte-routing";
  import { onMount } from 'svelte';
  import { auth } from './stores/auth.js';
  import { navigate } from 'svelte-routing';
  import Login from "./components/Login.svelte";
  import Dashboard from "./components/Dashboard.svelte";

  export let url = "";

  let isMenuOpen = false;

  onMount(() => {
    auth.init();
  });

  function handleLogout() {
    auth.logout();
    navigate('/login');
  }

  function toggleMenu() {
    isMenuOpen = !isMenuOpen;
  }
</script>

<Router {url}>
  <nav class="bg-blue-600 shadow-lg">
    <div class="max-w-6xl mx-auto px-4">
      <div class="flex justify-between">
        <div class="flex space-x-7">
          <div>
            <Link to="/" class="flex items-center py-4 px-2">
              <span class="font-semibold text-white text-lg">Air Quality Monitor</span>
            </Link>
          </div>
        </div>
        <div class="hidden md:flex items-center space-x-3">
          <Link to="/" class="py-4 px-2 text-white hover:text-gray-200 transition duration-300">Home</Link>
          {#if $auth.isAuthenticated}
            <Link to="/dashboard" class="py-4 px-2 text-white hover:text-gray-200 transition duration-300">Dashboard</Link>
            <span class="text-white">Welcome, {$auth.username}</span>
            <button on:click={handleLogout} class="py-2 px-4 bg-red-500 text-white rounded hover:bg-red-600 transition duration-300">Logout</button>
          {:else}
            <Link to="/login" class="py-2 px-4 bg-yellow-400 text-blue-600 rounded hover:bg-yellow-300 transition duration-300">Login</Link>
          {/if}
        </div>
        <div class="md:hidden flex items-center">
          <button on:click={toggleMenu} class="outline-none mobile-menu-button">
            <svg class="w-6 h-6 text-white hover:text-gray-200"
              x-show="!showMenu"
              fill="none"
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path d="M4 6h16M4 12h16M4 18h16"></path>
            </svg>
          </button>
        </div>
      </div>
    </div>
    <div class={isMenuOpen ? "mobile-menu" : "mobile-menu hidden md:hidden"}>
      <ul class="bg-blue-600">
        <li><Link to="/" class="block text-sm px-2 py-4 text-white hover:bg-blue-500 transition duration-300">Home</Link></li>
        {#if $auth.isAuthenticated}
          <li><Link to="/dashboard" class="block text-sm px-2 py-4 text-white hover:bg-blue-500 transition duration-300">Dashboard</Link></li>
          <li><button on:click={handleLogout} class="block w-full text-left text-sm px-2 py-4 text-white hover:bg-red-500 transition duration-300">Logout</button></li>
        {:else}
          <li><Link to="/login" class="block text-sm px-2 py-4 text-white hover:bg-yellow-400 transition duration-300">Login</Link></li>
        {/if}
      </ul>
    </div>
  </nav>

  <main class="container mx-auto mt-4 px-4">
    <Route path="/login" component={Login} />
    <Route path="/dashboard" component={Dashboard} />
    <Route path="/">
      <div class="text-center">
        <h1 class="text-3xl font-bold mb-4">Welcome to the Air Quality Monitoring System</h1>
        <p class="text-xl">Please login to view your dashboard.</p>
      </div>
    </Route>
  </main>
</Router>

<style global>
  @import 'https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css';
</style>
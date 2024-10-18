<script>
  import { onMount } from 'svelte';
  import axios from 'axios';
  import { auth } from '../stores/auth.js';
  import { navigate } from 'svelte-routing';
  import { Line } from 'svelte-chartjs';
  import { Chart, Title, Tooltip, Legend, LineElement, LinearScale, PointElement, CategoryScale } from 'chart.js';
  import { format, parseISO } from 'date-fns';

  Chart.register(Title, Tooltip, Legend, LineElement, LinearScale, PointElement, CategoryScale);

  let devices = [];
  let deviceData = [];
  let selectedMetric = 'temperature';
  let chartData = {
    labels: [],
    datasets: [
      {
        label: 'Temperature',
        data: [],
        borderColor: 'rgb(255, 99, 132)',
        backgroundColor: 'rgba(255, 99, 132, 0.5)',
      },
      {
        label: 'Humidity',
        data: [],
        borderColor: 'rgb(75, 192, 192)',
        backgroundColor: 'rgba(75, 192, 192, 0.5)',
      },
      {
        label: 'CO2',
        data: [],
        borderColor: 'rgb(153, 102, 255)',
        backgroundColor: 'rgba(153, 102, 255, 0.5)',
      },
    ],
  };
  let isFullScreen = false;
  let chartContainer;

  $: chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'top',
      },
      title: {
        display: true,
        text: 'Device Data Over Time',
      },
    },
    scales: {
      x: {
        title: {
          display: true,
          text: 'Time',
        },
      },
      y: {
        title: {
          display: true,
          text: 'Value',
        },
      },
    },
  };

  onMount(async () => {
    await fetchDevices();
    await fetchDeviceData();
  });

  async function fetchDevices() {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.get('https://h3-projekt2024.onrender.com/api/Devices', {
        headers: { Authorization: `Bearer ${token}` }
      });
      devices = response.data;
    } catch (error) {
      console.error('Error fetching devices:', error);
    }
  }

  async function fetchDeviceData() {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.get('https://h3-projekt2024.onrender.com/api/DeviceDatas', {
        headers: { Authorization: `Bearer ${token}` }
      });
      deviceData = response.data.map(data => ({
        ...data,
        createdAt: format(parseISO(data.createdAt), 'yyyy-MM-dd HH:mm:ss')
      }));
      updateChartData();
    } catch (error) {
      console.error('Error fetching device data:', error);
    }
  }

  function handleMetricChange(event) {
    selectedMetric = event.target.value;
    updateChartData();
  }

  function updateChartData() {
    chartData = {
      labels: deviceData.map(data => data.createdAt),
      datasets: [
        {
          label: 'Temperature',
          data: deviceData.map(data => data.temperature),
          borderColor: 'rgb(255, 99, 132)',
          backgroundColor: 'rgba(255, 99, 132, 0.5)',
          hidden: selectedMetric !== 'temperature',
        },
        {
          label: 'Humidity',
          data: deviceData.map(data => data.humidity),
          borderColor: 'rgb(75, 192, 192)',
          backgroundColor: 'rgba(75, 192, 192, 0.5)',
          hidden: selectedMetric !== 'humidity',
        },
        {
          label: 'CO2',
          data: deviceData.map(data => data.cO2),
          borderColor: 'rgb(153, 102, 255)',
          backgroundColor: 'rgba(153, 102, 255, 0.5)',
          hidden: selectedMetric !== 'cO2',
        },
      ],
    };
  }

  function toggleFullScreen() {
    if (!document.fullscreenElement) {
      if (chartContainer.requestFullscreen) {
        chartContainer.requestFullscreen();
      } else if (chartContainer.mozRequestFullScreen) { /* Firefox */
        chartContainer.mozRequestFullScreen();
      } else if (chartContainer.webkitRequestFullscreen) { /* Chrome, Safari & Opera */
        chartContainer.webkitRequestFullscreen();
      } else if (chartContainer.msRequestFullscreen) { /* IE/Edge */
        chartContainer.msRequestFullscreen();
      }
      isFullScreen = true;
    } else {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      } else if (document.mozCancelFullScreen) { /* Firefox */
        document.mozCancelFullScreen();
      } else if (document.webkitExitFullscreen) { /* Chrome, Safari & Opera */
        document.webkitExitFullscreen();
      } else if (document.msExitFullscreen) { /* IE/Edge */
        document.msExitFullscreen();
      }
      isFullScreen = false;
    }
  }
</script>

<main class="min-h-screen bg-gray-100 py-6 flex flex-col justify-center sm:py-12">
  <div class="relative py-3 sm:max-w-xl sm:mx-auto">
    <div class="absolute inset-0 bg-gradient-to-r from-cyan-400 to-light-blue-500 shadow-lg transform -skew-y-6 sm:skew-y-0 sm:-rotate-6 sm:rounded-3xl"></div>
    <div class="relative px-2 py-12 bg-white shadow-lg sm:rounded-3xl sm:p-10">
      <h1 class="text-4xl font-bold mb-8 text-center text-gray-800">Dashboard</h1>

      <div class="mb-8">
        <h2 class="text-2xl font-semibold mb-4 text-gray-700">Devices</h2>
        {#if devices.length > 0}
          <ul class="space-y-2">
            {#each devices as device}
              <li class="bg-gray-100 p-3 rounded-lg shadow-sm">{device.name}</li>
            {/each}
          </ul>
        {:else}
          <p class="text-gray-600 italic">No devices found.</p>
        {/if}
      </div>

      <div class="mb-8">
        <h2 class="text-2xl font-semibold mb-4 text-gray-700">Device Data Graph</h2>
        <div class="flex justify-between items-center mb-4">
          <div class="flex items-center">
            <label for="metric-select" class="mr-2 text-gray-700">Select Metric:</label>
            <select id="metric-select" on:change={handleMetricChange} class="form-select rounded-md shadow-sm border-gray-300 focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50">
              <option value="temperature">Temperature</option>
              <option value="humidity">Humidity</option>
              <option value="cO2">CO2</option>
            </select>
          </div>
          <button on:click={toggleFullScreen} class="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2">
            {isFullScreen ? 'Exit Full Screen' : 'Full Screen'}
          </button>
        </div>
        <div class="chart-container" bind:this={chartContainer}>
          <Line data={chartData} options={chartOptions} />
        </div>
      </div>

      <div>
        <h2 class="text-2xl font-semibold mb-4 text-gray-700">Device Data Table</h2>
        {#if deviceData.length > 0}
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Device ID</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Temperature</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Humidity</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">CO2</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Created At</th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                {#each deviceData as data}
                  <tr>
                    <td class="px-6 py-4 whitespace-nowrap">{data.temperature}</td>
                    <td class="px-6 py-4 whitespace-nowrap">{data.humidity}</td>
                    <td class="px-6 py-4 whitespace-nowrap">{data.cO2}</td>
                    <td class="px-6 py-4 whitespace-nowrap">{data.createdAt}</td>
                  </tr>
                {/each}
              </tbody>
            </table>
          </div>
        {:else}
          <p class="text-gray-600 italic">No device data found.</p>
        {/if}
      </div>
    </div>
  </div>
</main>

<style>
  .chart-container {
    height: 380px;
    width: 90%; 
    margin: auto; 
  }

  .chart-container:fullscreen {
    width: 100%; 
    height: 100%; 
    background: white;
    padding: 20px;
  }

  main {
    width: 100%; 
  }
</style>
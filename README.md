# Deal watchdog utility

A console application which monitors treading activity in real-time and log suspicious behavior.

## Usage

```console
watchdog --server 0.0.0.0 --login 1 --password userpassword --ratio 5 --delta 1 --verbose
```

```console
watchdog -s 0.0.0.0 -l 1 -p userpassword -ratio 5 -delta 1 -verbose
```

## Performance counters

For performance monitoring are used Windows performance counters.

- `Deal events from API to process` - Incoming deals queue counter

- `Deals to compare` - Work queue counter

## Development

### Scaling up and down

Scaling is possible via increasing/decreasing instances of components in [Startup.cs](src\Watchdog.App\Startup.cs class).

variables:

- ConsumersCount
- WatchersCount

### Workflow scheme

![Watchdog app cheme](docs/files/img/app-scheme.png)

### Technologies

## References

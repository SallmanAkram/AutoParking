%% Test Parallel Parking Fuzzy Logic
disp("=== Single Input Test ===");

% One test example
input = [4 1.5 15 0.3];   % [Front Rear Angle Offset]
output = evalfis(parkingFIS, input);

steering = output(1)
speed    = output(2)

disp("=== Multiple Scenario Test ===");

% Multiple test scenarios
inputs = [
    4     1.5    20    0.0;   % angled right
    4     1.5   -20    0.0;   % angled left
    4     0.5     0    0.0;   % too close to rear car
    0.3   4.0     0    0.0;   % too close to front car
    3     3       0    1.0;   % offset to the right
];

outputs = evalfis(parkingFIS, inputs);

steering_list = outputs(:,1);
speed_list    = outputs(:,2);

%% Create Results Table
T = table(inputs(:,1), inputs(:,2), inputs(:,3), inputs(:,4), ...
          steering_list, speed_list, ...
          'VariableNames', {'Front','Rear','Angle','Offset','SteerOut','SpeedOut'});

disp("=== Results Table ===");
disp(T);

%% Plot Outputs
figure;
subplot(2,1,1);
plot(steering_list, 'o-', 'LineWidth', 1.5);
title('Steering Output for Test Scenarios');
xlabel('Scenario Number');
ylabel('Steering Angle (deg)');
grid on;

subplot(2,1,2);
plot(speed_list, 'o-', 'LineWidth', 1.5);
title('Speed Output for Test Scenarios');
xlabel('Scenario Number');
ylabel('Speed (0–1)');
grid on;

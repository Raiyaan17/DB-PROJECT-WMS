const app = {
    state: {
        token: localStorage.getItem('token'),
        user: null, // Will decode from token
        currentView: 'dashboard',
        cart: [],
        enrolled: []
    },

    init: () => {
        if (!app.state.token) {
            window.location.href = 'login.html';
            return;
        }

        app.decodeUser();
        app.setupNavigation();
        app.setupLogout();
        app.setupBllToggle();
        app.loadDashboard();

        // Initial View
        app.navigateTo('dashboard');
    },

    setupBllToggle: () => {
        const toggle = document.getElementById('bll-toggle');
        const label = document.getElementById('bll-label');

        toggle.addEventListener('change', async () => {
            const mode = toggle.checked ? 'Sproc' : 'Linq';
            label.textContent = toggle.checked ? 'SP Mode' : 'LINQ Mode';

            // Call API to switch mode
            // Note: ConfigController is open, but we might want to secure it or just allow it for this demo
            // Since we are using app.api helper, it sends the token, so user must be logged in.
            const res = await app.api(`Config/bll-mode/${mode}`, 'POST');
            if (res && res.ok) {
                console.log(`Switched to ${mode} mode`);
                // Reload current view to reflect data from new source
                app.navigateTo(app.state.currentView);
            } else {
                alert('Failed to switch BLL mode');
                toggle.checked = !toggle.checked; // Revert
                label.textContent = toggle.checked ? 'SP Mode' : 'LINQ Mode';
            }
        });
    },

    decodeUser: async () => {
        try {
            const payload = JSON.parse(atob(app.state.token.split('.')[1]));
            app.state.user = {
                id: payload.nameid || payload.sub, // Adjust based on claim type
                role: payload.role
            };

            // Update UI
            document.getElementById('student-id').textContent = `ID: ${app.state.user.id}`;

            // Fetch Student Details
            const res = await app.api(`Students/${app.state.user.id}`);
            if (res && res.ok) {
                const student = await res.json();
                document.getElementById('student-name').textContent = `${student.fname} ${student.lname}`;
                document.getElementById('dash-school-name').textContent = student.schoolName || 'Unknown School';
            } else {
                document.getElementById('student-name').textContent = "Student";
            }

            // Set Date
            const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
            document.getElementById('current-date').textContent = new Date().toLocaleDateString('en-US', options);

        } catch (e) {
            console.error("Invalid token", e);
            app.logout();
        }
    },

    setupNavigation: () => {
        document.querySelectorAll('.nav-links li').forEach(item => {
            item.addEventListener('click', () => {
                const view = item.getAttribute('data-view');
                app.navigateTo(view);
            });
        });
    },

    setupLogout: () => {
        document.getElementById('logout-btn').addEventListener('click', app.logout);
    },

    logout: () => {
        localStorage.removeItem('token');
        window.location.href = 'login.html';
    },

    navigateTo: (viewName) => {
        // Update State
        app.state.currentView = viewName;

        // Update Nav UI
        document.querySelectorAll('.nav-links li').forEach(li => li.classList.remove('active'));
        const activeLink = document.querySelector(`.nav-links li[data-view="${viewName}"]`);
        if (activeLink) activeLink.classList.add('active');

        // Update View UI
        document.querySelectorAll('.view-section').forEach(el => el.classList.remove('active'));
        const activeView = document.getElementById(`view-${viewName}`);
        if (activeView) activeView.classList.add('active');

        // Load Data for View
        if (viewName === 'courses') app.loadCourses();
        if (viewName === 'cart') app.loadCart(); // Need to implement fetch cart logic if API supports it, or local
        if (viewName === 'schedule') app.loadSchedule();
        if (viewName === 'my-courses') app.loadEnrolled();
        if (viewName === 'dashboard') app.loadDashboard();
    },

    // --- API Helpers ---
    api: async (endpoint, method = 'GET', body = null) => {
        const headers = {
            'Authorization': `Bearer ${app.state.token}`,
            'Content-Type': 'application/json'
        };

        const config = { method, headers };
        if (body) config.body = JSON.stringify(body);

        try {
            const response = await fetch(`/api/${endpoint}`, config);
            if (response.status === 401) {
                app.logout();
                return null;
            }
            return response;
        } catch (err) {
            console.error("API Error", err);
            return null;
        }
    },

    // --- Feature: Dashboard ---
    loadDashboard: async () => {
        const studentId = app.state.user.id;

        // Fetch Enrolled Count and Chart Data
        const scheduleRes = await app.api(`Students/${studentId}/schedule`, 'GET');
        if (scheduleRes && scheduleRes.ok) {
            const schedule = await scheduleRes.json();
            const uniqueCourses = new Map();
            Object.values(schedule).flat().forEach(c => uniqueCourses.set(c.courseCode, c));

            document.getElementById('dash-enrolled-count').textContent = uniqueCourses.size;

            // Calculate Total Credits
            const totalCredits = Array.from(uniqueCourses.values()).reduce((sum, c) => sum + (c.totalCredits || 0), 0);
            document.getElementById('dash-credits-count').textContent = totalCredits;

            app.renderChart(Array.from(uniqueCourses.values()));
        }

        // Fetch Cart Count
        const cartRes = await app.api(`Students/${studentId}/cart`, 'GET');
        if (cartRes && cartRes.ok) {
            const cart = await cartRes.json();
            document.getElementById('dash-cart-count').textContent = cart.length;
        }
    },

    renderChart: (courses) => {
        const ctx = document.getElementById('enrollmentChart');
        if (!ctx) return;

        // Destroy existing chart if it exists to prevent overlap/memory leaks
        if (app.enrollmentChartInstance) {
            app.enrollmentChartInstance.destroy();
        }

        // Process data: Count courses per department
        const deptCounts = {};
        courses.forEach(c => {
            const dept = c.departmentName || 'Unknown';
            deptCounts[dept] = (deptCounts[dept] || 0) + 1;
        });

        const labels = Object.keys(deptCounts);
        const data = Object.values(deptCounts);

        // Generate colors
        const colors = [
            '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40',
            '#C9CBCF', '#FFCD56', '#4D5360', '#F7464A', '#46BFBD', '#FDB45C'
        ];

        app.enrollmentChartInstance = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: colors.slice(0, labels.length),
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right',
                    },
                    title: {
                        display: true,
                        text: 'Enrolled Courses by Department'
                    }
                }
            }
        });
    },

    // --- Feature: Courses ---
    loadCourses: async () => {
        console.log("Loading courses...");
        // Load Departments for Filter
        app.loadDepartments();

        // Setup Search Listener
        const searchBtn = document.getElementById('search-btn');
        if (searchBtn) searchBtn.onclick = app.searchCourses;

        const searchInput = document.getElementById('course-search');
        if (searchInput) {
            searchInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') app.searchCourses();
            });
        }

        // Fetch Enrolled Courses for Highlighting (Always fetch fresh)
        const studentId = app.state.user.id;
        try {
            const enrolledRes = await app.api(`Students/${studentId}/schedule`, 'GET');
            if (enrolledRes && enrolledRes.ok) {
                const schedule = await enrolledRes.json();
                app.state.enrolled = Object.values(schedule).flat().map(c => c.courseCode);
            }
        } catch (e) {
            console.error("Failed to fetch enrolled courses", e);
        }

        const res = await app.api('Courses');
        if (res && res.ok) {
            const courses = await res.json();
            app.renderCourses(courses);
        } else {
            console.error("Failed to load courses");
        }
    },

    searchCourses: async () => {
        const query = document.getElementById('course-search').value;
        const dept = document.getElementById('dept-filter').value;

        let endpoint = 'Courses';
        if (query) {
            endpoint = `Courses/search?q=${encodeURIComponent(query)}`;
        } else if (dept) {
            endpoint = `Courses/by-department/${encodeURIComponent(dept)}`;
        }

        const res = await app.api(endpoint);
        if (res && res.ok) {
            const courses = await res.json();
            app.renderCourses(courses);
        }
    },

    loadDepartments: async () => {
        const res = await app.api('Courses/departments');
        if (res && res.ok) {
            const depts = await res.json();
            const select = document.getElementById('dept-filter');
            if (!select) return;

            // Keep the first option (All Departments)
            select.innerHTML = '<option value="">All Departments</option>';

            depts.forEach(d => {
                const option = document.createElement('option');
                option.value = d.departmentId; // Use ID to match SP expectation
                option.textContent = d.departmentName;
                select.appendChild(option);
            });

            // Add change listener
            select.onchange = () => {
                const deptCode = select.value;
                if (deptCode) {
                    app.loadCoursesByDept(deptCode);
                } else {
                    app.loadCourses(); // Reload all
                }
            };
        }
    },

    loadCoursesByDept: async (deptCode) => {
        const res = await app.api(`Courses/by-department/${deptCode}`);
        if (res && res.ok) {
            const courses = await res.json();
            app.renderCourses(courses);
        }
    },

    renderCourses: (courses) => {
        const container = document.getElementById('course-list');
        if (!container) return;
        container.innerHTML = '';

        courses.forEach(c => {
            const isEnrolled = app.state.enrolled.includes(c.courseCode);
            const card = document.createElement('div');
            card.className = `course-card ${isEnrolled ? 'enrolled-course' : ''}`;
            card.innerHTML = `
                <div class="course-header">
                    <span class="course-code">${c.courseCode}</span>
                    <span class="course-credits">${c.totalCredits} Credits</span>
                </div>
                <h3 class="course-title">${c.courseTitle}</h3>
                <div class="course-details">
                    <p>👨‍🏫 ${c.instructorName || 'TBA'}</p>
                    <p>🕒 ${c.dayOfWeek || 'TBA'} ${c.startTime || ''} - ${c.endTime || ''}</p>
                    <p>👥 ${c.enrolledCount} / ${c.capacity}</p>
                    <p>${c.remainingSeats} seats remaining</p>
                </div>
                <div class="course-actions">
                    ${isEnrolled
                    ? '<span class="badge-enrolled">Enrolled</span>'
                    : `<button class="btn-secondary btn-sm" onclick="app.addToCart('${c.courseCode}')">Add to Cart</button>`
                }
                </div>
            `;
            container.appendChild(card);
        });
    },

    addToCart: async (courseCode) => {
        const studentId = app.state.user.id;
        const res = await app.api(`Students/${studentId}/cart/${courseCode}`, 'POST');
        if (res && res.ok) {
            alert('Added to cart!');
            app.loadDashboard(); // Update counts
        } else {
            const err = await res.text();
            alert(`Failed: ${err}`);
        }
    },

    // --- Feature: Cart ---
    loadCart: async () => {
        const studentId = app.state.user.id;
        const res = await app.api(`Students/${studentId}/cart`, 'GET');

        const container = document.getElementById('cart-list');
        container.innerHTML = '';

        if (res && res.ok) {
            const cartItems = await res.json();
            if (cartItems.length === 0) {
                container.innerHTML = '<p class="text-center">Your cart is empty.</p>';
            } else {
                cartItems.forEach(item => {
                    const card = document.createElement('div');
                    card.className = 'list-item';
                    // Update for CartItemDto: flat properties
                    const title = item.courseTitle || 'Unknown Course';
                    const credits = item.totalCredits || '?';

                    card.innerHTML = `
                        <div class="item-info">
                            <h4>${item.courseCode}</h4>
                            <p class="item-meta">${title} (${credits} Credits)</p>
                        </div>
                        <button class="btn-secondary" style="color: var(--error-color)" onclick="app.removeFromCart('${item.courseCode}')">Remove</button>
                    `;
                    container.appendChild(card);
                });
            }
        }

        document.getElementById('validate-cart-btn').onclick = app.validateCart;
        document.getElementById('enroll-cart-btn').onclick = app.enrollCart;
    },

    removeFromCart: async (courseCode) => {
        if (!confirm(`Remove ${courseCode} from cart?`)) return;

        const studentId = app.state.user.id;
        const res = await app.api(`Students/${studentId}/cart/${courseCode}`, 'DELETE');
        if (res && res.ok) {
            app.loadCart();
            app.loadDashboard();
        } else {
            alert('Failed to remove course.');
        }
    },

    validateCart: async () => {
        const studentId = app.state.user.id;
        const res = await app.api(`Students/${studentId}/cart/validate`, 'GET');
        const msgArea = document.getElementById('cart-messages');
        msgArea.innerHTML = '';

        if (res && res.ok) {
            const errors = await res.json();
            if (errors.length === 0) {
                msgArea.innerHTML = '<div class="alert success">Cart is valid! Ready to enroll.</div>';
            } else {
                msgArea.innerHTML = `<div class="alert error">Issues found:<ul>${errors.map(e => `<li>${e}</li>`).join('')}</ul></div>`;
            }
        }
    },

    enrollCart: async () => {
        const studentId = app.state.user.id;
        const res = await app.api(`Students/${studentId}/enroll`, 'POST');
        if (res && res.ok) {
            const msg = await res.text();
            alert(msg);
            // Refresh state before navigating
            await app.loadDashboard();
            // Also refresh enrolled list in state for highlighting
            const enrolledRes = await app.api(`Students/${studentId}/schedule`, 'GET');
            if (enrolledRes && enrolledRes.ok) {
                const schedule = await enrolledRes.json();
                app.state.enrolled = Object.values(schedule).flat().map(c => c.courseCode);
            }
            app.navigateTo('my-courses');
        } else {
            const err = await res.text();
            alert(`Enrollment Failed: ${err}`);
        }
    },

    // --- Feature: Schedule ---
    loadSchedule: async () => {
        const studentId = app.state.user.id;
        const res = await app.api(`Students/${studentId}/schedule`, 'GET');
        if (res && res.ok) {
            const schedule = await res.json();
            app.renderSchedule(schedule);
        }
    },

    renderSchedule: (scheduleMap) => {
        const container = document.getElementById('schedule-grid');
        container.innerHTML = '';

        // 1. Render Headers
        const days = ['Time', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'];
        days.forEach(day => {
            const header = document.createElement('div');
            header.className = 'cal-header';
            header.textContent = day;
            container.appendChild(header);
        });

        // 2. Render Time Slots (Background Grid)
        // 8:00 AM to 9:00 PM (13 hours * 2 slots = 26 slots)
        const startHour = 8;
        const endHour = 21;

        for (let h = startHour; h < endHour; h++) {
            // Time Label
            const timeLabel = document.createElement('div');
            timeLabel.className = 'cal-time-col';
            timeLabel.style.gridRow = `${(h - startHour) * 2 + 2}`; // +2 for header offset
            const displayTime = h > 12 ? `${h - 12} PM` : (h === 12 ? '12 PM' : `${h} AM`);
            timeLabel.textContent = displayTime;
            container.appendChild(timeLabel);

            // Empty Cells for days
            for (let d = 0; d < 5; d++) {
                const cell = document.createElement('div');
                cell.className = 'cal-cell';
                cell.style.gridRow = `${(h - startHour) * 2 + 2} / span 2`;
                cell.style.gridColumn = d + 2;
                container.appendChild(cell);
            }
        }

        // 3. Render Courses
        const dayMap = { 'Monday': 2, 'Tuesday': 3, 'Wednesday': 4, 'Thursday': 5, 'Friday': 6 };

        Object.values(scheduleMap).flat().forEach(c => {
            if (!c.dayOfWeek || !c.startTime || !c.endTime) return;

            // Parse Days (e.g., "MW" -> Monday, Wednesday)
            const days = [];
            if (c.dayOfWeek.includes('M')) days.push('Monday');
            if (c.dayOfWeek.includes('T') && !c.dayOfWeek.includes('Th')) days.push('Tuesday'); // Simple check, might need refinement for 'Tu'
            if (c.dayOfWeek.includes('W')) days.push('Wednesday');
            if (c.dayOfWeek.includes('Th')) days.push('Thursday');
            if (c.dayOfWeek.includes('F')) days.push('Friday');

            // Parse Time
            const parseTime = (t) => {
                const [h, m] = t.split(':').map(Number);
                return h + (m / 60);
            };

            const start = parseTime(c.startTime);
            const end = parseTime(c.endTime);

            // Calculate Grid Position (30min slots)
            // Row 1 is Header
            // Row 2 starts at 8:00 AM
            const rowStart = Math.round((start - startHour) * 2) + 2;
            const durationSlots = Math.round((end - start) * 2);

            days.forEach(dayName => {
                const col = dayMap[dayName];
                if (!col) return;

                const event = document.createElement('div');
                event.className = 'cal-event';
                event.style.gridColumn = col;
                event.style.gridRow = `${rowStart} / span ${durationSlots}`;
                event.innerHTML = `
                    <strong>${c.courseCode}</strong>
                    <div style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">${c.courseTitle}</div>
                    <div>${c.venue || ''}</div>
                `;
                event.title = `${c.courseTitle}\n${c.startTime} - ${c.endTime}`;
                container.appendChild(event);
            });
        });
    },

    // --- Feature: My Courses ---
    loadEnrolled: async () => {
        // We don't have a direct "Get Enrolled Courses" endpoint that returns list
        // We have GetSchedule which returns them by day. 
        // We can use GetSchedule and deduplicate to show list.
        const studentId = app.state.user.id;
        const res = await app.api(`Students/${studentId}/schedule`, 'GET');
        if (res && res.ok) {
            const schedule = await res.json();
            const uniqueCourses = new Map();

            Object.values(schedule).flat().forEach(c => {
                if (!uniqueCourses.has(c.courseCode)) {
                    uniqueCourses.set(c.courseCode, c);
                }
            });

            app.renderEnrolled(Array.from(uniqueCourses.values()));
        }
    },

    renderEnrolled: (courses) => {
        const container = document.getElementById('enrolled-list');
        container.innerHTML = '';

        courses.forEach(c => {
            const item = document.createElement('div');
            item.className = 'list-item';
            item.innerHTML = `
                <div class="item-info">
                    <h4>${c.courseCode}: ${c.courseTitle}</h4>
                    <p class="item-meta">
                        ${c.dayOfWeek || 'TBA'} ${c.startTime || ''} - ${c.endTime || ''} 
                        ${c.venue ? `| Room: ${c.venue}` : ''}
                    </p>
                </div>
                <button class="btn-secondary" style="color: var(--error-color)" onclick="app.dropCourse('${c.courseCode}')">Drop</button>
            `;
            container.appendChild(item);
        });

        document.getElementById('dash-enrolled-count').textContent = courses.length;
    },

    dropCourse: async (courseCode) => {
        if (!confirm(`Are you sure you want to drop ${courseCode}?`)) return;

        const studentId = app.state.user.id;
        const res = await app.api(`Students/${studentId}/enroll/${courseCode}`, 'DELETE');
        if (res && res.ok) {
            alert('Course dropped.');
            app.loadEnrolled();
        } else {
            alert('Failed to drop course.');
        }
    }
};

// Start App
document.addEventListener('DOMContentLoaded', app.init);

const admin = {
    state: {
        token: localStorage.getItem('token'),
        user: null,
        currentView: 'dashboard'
    },

    init: async () => {
        // Auth Check
        if (!admin.state.token) {
            window.location.href = 'login.html';
            return;
        }

        // Decode Token to check role
        const payload = JSON.parse(atob(admin.state.token.split('.')[1]));
        const userRole = payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

        if (!userRole || userRole.toLowerCase() !== 'admin') {
            alert('Access Denied: You are not an specific admin.');
            console.error('Expected admin, got:', userRole);
            window.location.href = 'index.html';
            return;
        }

        admin.state.user = {
            id: payload.nameid || payload.sub,
            role: userRole
        };

        // Navigation Setup
        document.querySelectorAll('.nav-links li').forEach(item => {
            item.addEventListener('click', () => {
                const view = item.getAttribute('data-view');
                admin.navigateTo(view);
            });
        });

        document.getElementById('logout-btn').addEventListener('click', () => {
            localStorage.removeItem('token');
            localStorage.removeItem('userRole');
            window.location.href = 'login.html';
        });

        // Date Display
        const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
        document.getElementById('current-date').textContent = new Date().toLocaleDateString('en-US', options);

        // BLL Toggle Setup
        admin.setupBllToggle();

        // Initial Load
        admin.navigateTo('dashboard');
    },

    setupBllToggle: () => {
        const toggle = document.getElementById('bll-toggle');
        const label = document.getElementById('bll-label');

        toggle.addEventListener('change', async () => {
            const mode = toggle.checked ? 'Sproc' : 'Linq';
            label.textContent = toggle.checked ? 'SP Mode' : 'LINQ Mode';

            // Call API to switch mode
            const res = await admin.api(`Config/bll-mode/${mode}`, 'POST');
            if (res && res.ok) {
                console.log(`Switched to ${mode} mode`);
                // Reload current view to reflect data from new source
                admin.navigateTo(admin.state.currentView);
            } else {
                alert('Failed to switch BLL mode');
                toggle.checked = !toggle.checked; // Revert
                label.textContent = toggle.checked ? 'SP Mode' : 'LINQ Mode';
            }
        });
    },

    navigateTo: (viewName) => {
        admin.state.currentView = viewName;

        // UI Updates
        document.querySelectorAll('.nav-links li').forEach(li => li.classList.remove('active'));
        const activeLink = document.querySelector(`.nav-links li[data-view="${viewName}"]`);
        if (activeLink) activeLink.classList.add('active');

        document.querySelectorAll('.view-section').forEach(el => el.classList.remove('active'));
        const activeView = document.getElementById(`view-${viewName}`);
        if (activeView) activeView.classList.add('active');

        // Data Loading
        if (viewName === 'dashboard') admin.loadDashboard();
        if (viewName === 'students') admin.loadStudents();
        if (viewName === 'instructors') admin.loadInstructors();
        if (viewName === 'courses') admin.loadCourses();
        if (viewName === 'archive') admin.loadArchive();
    },

    // --- API Helper ---
    api: async (endpoint, method = 'GET', body = null) => {
        const headers = {
            'Authorization': `Bearer ${admin.state.token}`,
            'Content-Type': 'application/json'
        };

        const config = { method, headers };
        if (body) config.body = JSON.stringify(body);

        try {
            const response = await fetch(`/api/${endpoint}`, config);
            if (response.status === 401) {
                window.location.href = 'login.html';
                return null;
            }
            return response;
        } catch (err) {
            console.error("API Error", err);
            return null;
        }
    },

    // --- Archive ---
    loadArchive: async () => {
        const res = await admin.api('Admin/archive/years');
        const select = document.getElementById('archive-year-filter');
        // Clear except first option
        select.innerHTML = '<option value="">Select Year</option>';

        if (res && res.ok) {
            const years = await res.json();
            years.forEach(y => {
                const opt = document.createElement('option');
                opt.value = y;
                opt.textContent = y;
                select.appendChild(opt);
            });
        }
    },

    loadArchivedStudents: async (year) => {
        const tbody = document.getElementById('archive-table-body');
        tbody.innerHTML = '<tr><td colspan="5">Loading...</td></tr>';

        if (!year) {
            tbody.innerHTML = '<tr><td colspan="5">Please select a year.</td></tr>';
            return;
        }

        const res = await admin.api(`Admin/archive/students/${year}`);
        if (res && res.ok) {
            const students = await res.json();
            tbody.innerHTML = '';
            if (students.length === 0) {
                tbody.innerHTML = '<tr><td colspan="5">No archived students found for this year.</td></tr>';
            } else {
                students.forEach(s => {
                    const tr = document.createElement('tr');
                    tr.innerHTML = `
                        <td>${s.studentId}</td>
                        <td>${s.fName} ${s.lName}</td>
                        <td>${s.email}</td>
                        <td>${s.departmentId}</td>
                        <td>${s.graduationYear}</td>
                    `;
                    tbody.appendChild(tr);
                });
            }
        } else {
            tbody.innerHTML = '<tr><td colspan="5">Error loading data.</td></tr>';
        }
    },

    // --- Dashboard ---
    loadDashboard: async () => {
        // Parallel fetch for counts if possible, or sequential

        // Fetch Total Student Count (filtered)
        const studentsCountRes = await admin.api('Admin/students/count');
        if (studentsCountRes && studentsCountRes.ok) {
            const count = await studentsCountRes.json();
            document.getElementById('stats-students').textContent = count;
        }

        const instRes = await admin.api('Admin/instructors');
        if (instRes && instRes.ok) {
            const inst = await instRes.json();
            document.getElementById('stats-instructors').textContent = inst.length;
        }

        const coursesRes = await admin.api('Admin/courses');
        if (coursesRes && coursesRes.ok) {
            const courses = await coursesRes.json();
            const activeCount = courses.filter(c => c.isActive).length;
            document.getElementById('stats-courses').textContent = activeCount;
        }
    },

    // --- Students ---
    loadStudents: async () => {
        const res = await admin.api('Admin/students');
        const tbody = document.getElementById('students-table-body');
        tbody.innerHTML = '<tr><td colspan="5">Loading...</td></tr>';

        if (res && res.ok) {
            const students = await res.json();
            tbody.innerHTML = '';
            students.forEach(s => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${s.studentId}</td>
                    <td>${s.fName} ${s.lName}</td>
                    <td>${s.email}</td>
                    <td>${s.departmentId}</td>
                    <td>${s.graduationYear}</td>
                `;
                tbody.appendChild(tr);
            });
        }
    },

    handleAddStudent: async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        // Convert year to int
        data.GraduationYear = parseInt(data.GraduationYear);

        const res = await admin.api('Admin/students', 'POST', data);
        if (res && res.ok) {
            alert('Student added successfully');
            admin.closeModal('modal-add-student');
            e.target.reset();
            admin.loadStudents();
        } else {
            const err = await res.text();
            alert(`Error: ${err}`);
        }
    },

    // --- Instructors ---
    loadInstructors: async () => {
        // Populate filter if empty
        const filterSelect = document.getElementById('instructor-dept-filter');
        if (filterSelect && filterSelect.options.length <= 1) {
            const deptRes = await admin.api('Admin/departments');
            if (deptRes && deptRes.ok) {
                const depts = await deptRes.json();
                depts.forEach(d => {
                    const opt = document.createElement('option');
                    opt.value = d;
                    opt.textContent = d;
                    filterSelect.appendChild(opt);
                });
            }
        }

        const deptId = filterSelect ? filterSelect.value : '';
        const url = deptId ? `Admin/instructors?departmentId=${deptId}` : 'Admin/instructors';

        const res = await admin.api(url);
        const tbody = document.getElementById('instructors-table-body');
        tbody.innerHTML = '<tr><td colspan="4">Loading...</td></tr>';

        if (res && res.ok) {
            const instructors = await res.json();
            tbody.innerHTML = '';
            instructors.forEach(i => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${i.instructorId}</td>
                    <td>${i.fName} ${i.lName}</td>
                    <td>${i.email}</td>
                    <td>${i.departmentId}</td>
                `;
                tbody.appendChild(tr);
            });
        }
    },

    handleAddInstructor: async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        const res = await admin.api('Admin/instructors', 'POST', data);
        if (res && res.ok) {
            alert('Instructor added successfully');
            admin.closeModal('modal-add-instructor');
            e.target.reset();
            admin.loadInstructors();
        } else {
            const err = await res.text();
            alert(`Error: ${err}`);
        }
    },

    // --- Courses ---
    loadCourses: async () => {
        // Populate filter if empty
        const filterSelect = document.getElementById('course-dept-filter');
        if (filterSelect && filterSelect.options.length <= 1) {
            const deptRes = await admin.api('Admin/departments');
            if (deptRes && deptRes.ok) {
                const depts = await deptRes.json();
                depts.forEach(d => {
                    const opt = document.createElement('option');
                    opt.value = d;
                    opt.textContent = d;
                    filterSelect.appendChild(opt);
                });
            }
        }

        const deptId = filterSelect ? filterSelect.value : '';
        const url = deptId ? `Admin/courses?departmentId=${deptId}` : 'Admin/courses';

        const res = await admin.api(url);
        const tbody = document.getElementById('courses-table-body');
        tbody.innerHTML = '<tr><td colspan="6">Loading...</td></tr>';

        if (res && res.ok) {
            const courses = await res.json();
            tbody.innerHTML = '';
            courses.forEach(c => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${c.courseCode}</td>
                    <td>${c.courseTitle}</td>
                    <td>${c.totalCredits}</td>
                    <td>${c.enrolledCount} / ${c.capacity}</td>
                    <td>${c.isActive ? 'Active' : 'Inactive'}</td>
                    <td>
                        <button onclick="admin.updateCapacity('${c.courseCode}', ${c.capacity})">Cap</button>
                        <button onclick="admin.toggleStatus('${c.courseCode}', ${!c.isActive})">
                            ${c.isActive ? 'Deactivate' : 'Activate'}
                        </button>
                    </td>
                `;
                tbody.appendChild(tr);
            });
        }
    },

    handleAddCourse: async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        // Conversions
        data.TotalCredits = parseInt(data.TotalCredits);
        data.Capacity = parseInt(data.Capacity);
        // Time format usually needs to be HH:mm:ss for backend span, but basic string might work depending on binding.
        // If optional fields are empty string, set them to null
        if (!data.InstructorId) data.InstructorId = null;
        if (!data.Venue) data.Venue = null;
        if (!data.DayOfWeek) data.DayOfWeek = null;
        if (!data.StartTime) data.StartTime = null; else data.StartTime += ":00"; // Add seconds
        if (!data.EndTime) data.EndTime = null; else data.EndTime += ":00";

        const res = await admin.api('Admin/courses', 'POST', data);
        if (res && res.ok) {
            alert('Course added successfully');
            admin.closeModal('modal-add-course');
            e.target.reset();
            admin.loadCourses();
        } else {
            const err = await res.text();
            alert(`Error: ${err}`);
        }
    },

    updateCapacity: async (courseCode, currentCap) => {
        const newCap = prompt(`Enter new capacity for ${courseCode}:`, currentCap);
        if (newCap && !isNaN(newCap) && newCap != currentCap) {
            const res = await admin.api(`Admin/courses/${courseCode}/capacity`, 'PATCH', { Capacity: parseInt(newCap) });
            if (res && res.ok) {
                alert('Capacity updated');
                admin.loadCourses();
            } else {
                alert('Failed to update capacity');
            }
        }
    },

    toggleStatus: async (courseCode, currentStatus) => {
        const newStatus = !currentStatus;
        if (confirm(`Are you sure you want to ${newStatus ? 'enable' : 'disable'} ${courseCode}?`)) {
            const res = await admin.api(`Admin/courses/${courseCode}/status`, 'PATCH', { IsActive: newStatus });
            if (res && res.ok) {
                alert('Status updated');
                admin.loadCourses();
            } else {
                alert('Failed to update status');
            }
        }
    },

    // --- Operations ---
    handleWaitlist: async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        const res = await admin.api('Admin/waitlist', 'POST', data);
        if (res && res.ok) {
            alert('Student waitlisted successfully');
            e.target.reset();
        } else {
            const err = await res.text();
            alert(`Error: ${err}`);
        }
    },

    handleForceEnroll: async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        const res = await admin.api('Admin/force-enroll', 'POST', data);
        if (res && res.ok) {
            alert('Student force enrolled successfully');
            e.target.reset();
        } else {
            const err = await res.text();
            alert(`Error: ${err}`);
        }
    },

    handleCompletion: async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());
        data.Completed = data.completed === 'true'; // Convert string to bool

        const res = await admin.api('Admin/enrollments/completion', 'POST', data);
        if (res && res.ok) {
            alert('Enrollment completion updated');
            e.target.reset();
        } else {
            const err = await res.text();
            alert(`Error: ${err}`);
        }
    },

    // --- Modal Utils ---
    openModal: (id) => {
        document.getElementById(id).classList.add('open');
    },

    closeModal: (id) => {
        document.getElementById(id).classList.remove('open');
    }
};

document.addEventListener('DOMContentLoaded', admin.init);

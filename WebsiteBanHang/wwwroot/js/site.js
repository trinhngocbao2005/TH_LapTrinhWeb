// Ngọc Bảo Studio - Premium UI/UX Animation & Interaction Engine
document.addEventListener('DOMContentLoaded', () => {
    initThemeSwitcher();
    init3DTiltCards();
    initHeroMouseParallax();
    initMicroInteractions();
});


/**
 * 1. Dynamic 3D Tilt Hover Effect for Premium Product Cards
 */
function init3DTiltCards() {
    const cards = document.querySelectorAll('.product-card-premium');
    
    cards.forEach(card => {
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left; // Mouse position inside card
            const y = e.clientY - rect.top;
            
            const centerX = rect.width / 2;
            const centerY = rect.height / 2;
            
            // Calculate rotation offsets (Max 8 degrees tilt for professional feel)
            const rotateX = ((centerY - y) / centerY) * 7;
            const rotateY = ((x - centerX) / centerX) * 7;
            
            // Apply 3D perspective transform
            card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) scale3d(1.025, 1.025, 1.025)`;
            
            // Dynamically tilt the float shadow slightly opposite
            const shadow = card.querySelector('.floating-shadow');
            if (shadow) {
                const shiftX = ((x - centerX) / centerX) * -8;
                shadow.style.transform = `translateZ(10px) translateX(${shiftX}px)`;
            }
        });
        
        card.addEventListener('mouseleave', () => {
            // Restore neutral 3D position
            card.style.transform = 'perspective(1000px) rotateX(0deg) rotateY(0deg) scale3d(1, 1, 1)';
            
            const shadow = card.querySelector('.floating-shadow');
            if (shadow) {
                shadow.style.transform = 'translateZ(10px) translateX(0px)';
            }
        });
    });
}

/**
 * 2. Mouse Tracking 3D Parallax Drift for Hero Banner
 */
function initHeroMouseParallax() {
    const banner = document.querySelector('.studio-hub-banner-container');
    if (!banner) return;
    
    banner.addEventListener('mousemove', (e) => {
        const rect = banner.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        
        const centerX = rect.width / 2;
        const centerY = rect.height / 2;
        
        // Multi-layered depth factor
        const moveX = (x - centerX) / centerX * 16;
        const moveY = (y - centerY) / centerY * 12;
        
        const badge = banner.querySelector('.studio-hero-badge');
        const title = banner.querySelector('.hero-main-title');
        const desc = banner.querySelector('.hero-description');
        const rowBadges = banner.querySelector('.border-top.border-bottom');
        
        // Apply differential translations to evoke 3D depth of layers
        if (badge) {
            badge.style.transform = `translate3d(${moveX * -0.3}px, ${moveY * -0.3}px, 0)`;
            badge.style.transition = 'transform 0.1s ease-out';
        }
        if (title) {
            title.style.transform = `translate3d(${moveX * 0.25}px, ${moveY * 0.2}px, 0)`;
            title.style.transition = 'transform 0.1s ease-out';
        }
        if (desc) {
            desc.style.transform = `translate3d(${moveX * 0.15}px, ${moveY * 0.1}px, 0)`;
            desc.style.transition = 'transform 0.1s ease-out';
        }
        if (rowBadges) {
            rowBadges.style.transform = `translate3d(${moveX * 0.08}px, ${moveY * 0.05}px, 0)`;
            rowBadges.style.transition = 'transform 0.1s ease-out';
        }
    });
    
    banner.addEventListener('mouseleave', () => {
        const badge = banner.querySelector('.studio-hero-badge');
        const title = banner.querySelector('.hero-main-title');
        const desc = banner.querySelector('.hero-description');
        const rowBadges = banner.querySelector('.border-top.border-bottom');
        
        // Smoothly restore neutral translation state
        const elements = [badge, title, desc, rowBadges];
        elements.forEach(el => {
            if (el) {
                el.style.transform = 'translate3d(0, 0, 0)';
                el.style.transition = 'transform 0.6s cubic-bezier(0.165, 0.84, 0.44, 1)';
            }
        });
    });
}

/**
 * 3. Micro-interactions for Action Buttons
 */
function initMicroInteractions() {
    // Add magnetic hover feel or visual click ripples to premium buttons if necessary
    const primaryBtns = document.querySelectorAll('.btn-premium-primary, .btn-premium-outline');
    primaryBtns.forEach(btn => {
        btn.addEventListener('mousedown', function(e) {
            const circle = document.createElement('div');
            const d = Math.max(this.clientWidth, this.clientHeight);
            
            circle.style.width = circle.style.height = d + 'px';
            circle.style.left = e.clientX - this.getBoundingClientRect().left - d/2 + 'px';
            circle.style.top = e.clientY - this.getBoundingClientRect().top - d/2 + 'px';
            circle.style.position = 'absolute';
            circle.style.borderRadius = '50%';
            circle.style.background = 'rgba(255, 255, 255, 0.35)';
            circle.style.transform = 'scale(0)';
            circle.style.animation = 'ripple 0.6s linear';
            circle.style.pointerEvents = 'none';
            
            this.style.position = 'relative';
            this.style.overflow = 'hidden';
            this.appendChild(circle);
            
            setTimeout(() => { circle.remove(); }, 600);
        });
    });
}

/**
 * 4. Premium Dark/Light Theme Switching Engine
 */
function initThemeSwitcher() {
    const currentTheme = localStorage.getItem('theme') || 'light';
    applyTheme(currentTheme);
}

function applyTheme(theme) {
    const sunIcon = document.querySelector('.theme-icon-light');
    const moonIcon = document.querySelector('.theme-icon-dark');
    
    if (theme === 'dark') {
        document.documentElement.classList.add('dark-theme');
        document.body.classList.add('dark-theme');
        if (sunIcon) sunIcon.classList.remove('d-none');
        if (moonIcon) moonIcon.classList.add('d-none');
    } else {
        document.documentElement.classList.remove('dark-theme');
        document.body.classList.remove('dark-theme');
        if (sunIcon) sunIcon.classList.add('d-none');
        if (moonIcon) moonIcon.classList.remove('d-none');
    }
}

window.toggleTheme = function() {
    const currentTheme = localStorage.getItem('theme') === 'dark' ? 'light' : 'dark';
    localStorage.setItem('theme', currentTheme);
    applyTheme(currentTheme);
};

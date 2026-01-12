'use client';

import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Button,
  Box,
  Grid,
  Alert,
  Snackbar,
  AppBar,
  Toolbar,
  Paper,
} from '@mui/material';
import {
  Add as AddIcon,
  Refresh as RefreshIcon,
  Compare as CompareIcon,
} from '@mui/icons-material';
import VendorList from '../components/VendorList';
import VendorFormModal from '../components/VendorFormModal';
import VendorDetails from '../components/VendorDetails';
import RiskAssessmentModal from '../components/RiskAssessmentModal';
import VendorComparisonModal from '../components/VendorComparisonModal';
import { Vendor, CreateVendorRequest } from '../types/vendor';
import api from '../services/api';

export default function Home() {
  const [vendors, setVendors] = useState<Vendor[]>([]);
  const [selectedVendor, setSelectedVendor] = useState<Vendor | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Modal states
  const [formModalOpen, setFormModalOpen] = useState(false);
  const [riskModalOpen, setRiskModalOpen] = useState(false);
  const [comparisonModalOpen, setComparisonModalOpen] = useState(false);

  // Snackbar state
  const [snackbar, setSnackbar] = useState<{
    open: boolean;
    message: string;
    severity: 'success' | 'error' | 'info';
  }>({
    open: false,
    message: '',
    severity: 'success',
  });

  useEffect(() => {
    fetchVendors();
  }, []);

  const fetchVendors = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await api.getVendors();
      setVendors(data);
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Failed to fetch vendors';
      setError(errorMessage);
      showSnackbar(errorMessage, 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateVendor = async (data: CreateVendorRequest) => {
    try {
      await api.createVendor(data);
      showSnackbar('Vendor created successfully', 'success');
      await fetchVendors();
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Failed to create vendor';
      showSnackbar(errorMessage, 'error');
      throw err;
    }
  };

  const handleViewDetails = (vendor: Vendor) => {
    setSelectedVendor(vendor);
  };

  const handleViewRisk = (vendor: Vendor) => {
    setSelectedVendor(vendor);
    setRiskModalOpen(true);
  };

  const showSnackbar = (message: string, severity: 'success' | 'error' | 'info') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleCloseSnackbar = () => {
    setSnackbar({ ...snackbar, open: false });
  };

  return (
    <>
      <AppBar position="static" elevation={0}>
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1, fontWeight: 600 }}>
            Vendor Risk Scoring System
          </Typography>
        </Toolbar>
      </AppBar>

      <Container maxWidth="xl" sx={{ mt: { xs: 2, sm: 4 }, mb: 4, px: { xs: 2, sm: 3 } }}>
        <Box sx={{
          mb: 3,
          display: 'flex',
          flexDirection: { xs: 'column', sm: 'row' },
          justifyContent: 'space-between',
          alignItems: { xs: 'stretch', sm: 'center' },
          gap: 2
        }}>
          <Typography variant="h4" fontWeight={600} sx={{ mb: { xs: 1, sm: 0 } }}>
            Vendor Management
          </Typography>
          <Box sx={{
            display: 'flex',
            flexDirection: { xs: 'column', sm: 'row' },
            gap: 1.5
          }}>
            <Button
              variant="outlined"
              startIcon={<RefreshIcon />}
              onClick={fetchVendors}
              disabled={loading}
              sx={{ width: { xs: '100%', sm: 'auto' } }}
            >
              Refresh
            </Button>
            <Button
              variant="outlined"
              startIcon={<CompareIcon />}
              onClick={() => setComparisonModalOpen(true)}
              disabled={vendors.length < 2}
              sx={{ width: { xs: '100%', sm: 'auto' } }}
            >
              Compare
            </Button>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => setFormModalOpen(true)}
              sx={{ width: { xs: '100%', sm: 'auto' } }}
            >
              Create
            </Button>
          </Box>
        </Box>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError(null)}>
            {error}
          </Alert>
        )}

        <Grid container spacing={{ xs: 2, sm: 3 }}>
          <Grid size={{ xs: 12, lg: 8 }}>
            <Paper sx={{ p: { xs: 2, sm: 3 } }}>
              <Typography variant="h6" gutterBottom fontWeight={600} sx={{ mb: 2, fontSize: { xs: '1.1rem', sm: '1.25rem' } }}>
                Vendors List
              </Typography>
              {loading ? (
                <Typography variant="body2" color="text.secondary" align="center" sx={{ py: 4 }}>
                  Loading vendors...
                </Typography>
              ) : (
                <VendorList
                  vendors={vendors}
                  onViewDetails={handleViewDetails}
                  onViewRisk={handleViewRisk}
                />
              )}
            </Paper>
          </Grid>

          <Grid size={{ xs: 12, lg: 4 }}>
            <Box sx={{ position: { xs: 'relative', lg: 'sticky' }, top: 16 }}>
              <Typography variant="h6" gutterBottom fontWeight={600} sx={{ mb: 2 }}>
                Vendor Details
              </Typography>
              <VendorDetails vendor={selectedVendor} />
            </Box>
          </Grid>
        </Grid>
      </Container>

      <VendorFormModal
        open={formModalOpen}
        onClose={() => setFormModalOpen(false)}
        onSubmit={handleCreateVendor}
      />

      <RiskAssessmentModal
        open={riskModalOpen}
        onClose={() => setRiskModalOpen(false)}
        vendorId={selectedVendor?.id || null}
        vendorName={selectedVendor?.name || ''}
      />

      <VendorComparisonModal
        open={comparisonModalOpen}
        onClose={() => setComparisonModalOpen(false)}
        vendors={vendors}
      />

      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert
          onClose={handleCloseSnackbar}
          severity={snackbar.severity}
          sx={{ width: '100%' }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </>
  );
}
